<?php
require_once("gameData.php");
require_once("playerCurGameData.php");
require_once("packetID.php");
require_once("../serverCommon/packet.php");


class GameState
{
    public const NONE = 0;
    public const GAMMING = 1;
    public const GAME_RESULT = 2;
}

class DualGoGame
{
    const TURN_WAIT_TIME = 10;
    const GAME_RESULT_WAIT_TIME = 10;

    public $SendPacketFunc;
    public $BroadcastSendPacketFunc;

    private int $State = GameState::NONE;

    private int $CurDummyPaePos;
    private array $DummyPaesNum;

    private int $CurTurnPlayerIndex;

    private PlayerCurGameData $Player1;
    private PlayerCurGameData $Player2;

    private array $BadagPaes;

    private int $WaitLimitTimeTurn;
    private int $WaitLimitTimeEndGameResult;


    public function __construct()
    {
        for($i = 0; $i < HwatuInfo::MaxPaeCount; ++$i)
        {
            $this->DummyPaesNum[$i] = $i;
        }

        $this->Player1 = new PlayerCurGameData();
        $this->Player2 = new PlayerCurGameData();

        $this->Clear();
    }

    public function Clear()
    {
        $this->State = GameState::NONE;
        $this->WaitLimitTimeEndGameResult = 0;
        $this->WaitLimitTimeTurn = 0;

        $this->CurDummyPaePos = 0;
        $this->BadagPaes = array();

        $this->Player1->Clear();
        $this->Player2->Clear();
    }

    public function EndGame()
    {
        $this->State = GameState::GAME_RESULT;
        $this->WaitLimitTimeEndGameResult = time() + self::GAME_RESULT_WAIT_TIME;
    }

    public function IsGaming() : bool { return $this->State == GameState::GAMMING; }

    public function EnableGameStart() : bool { return $this->State == GameState::NONE ? true : false; }

    public function IsTurnPlayer(string $userID) : bool
    {
        return $this->GetPlayerData($this->CurTurnPlayerIndex)->UserID == $userID;
    }


    public function Update(int $curTime)
    {
        if($this->State == GameState::NONE)
        {
            return;
        }

        if($this->State == GameState::GAME_RESULT)
        {
            if($this->WaitLimitTimeEndGameResult <= $curTime)
            {
                $this->Clear();
            }
            return;
        }
        else if($this->State == GameState::GAMMING)
        {
            if($this->WaitLimitTimeTurn > $curTime)
            {
                return;
            }

            $this->AutoPlayerTurnThrowPae();
        }
    }

    public function StartGame(string $p1UserID, string $p2UserID)
    {
        $this->ShuffleDummyPaes();
        $this->CurDummyPaePos = 0;

        for($i = 0; $i < 10; ++$i)
        {
            $pae = $this->DummyPaesNum[$this->CurDummyPaePos];
            $this->Player1->HoldPaes[$pae] = $pae;
            ++$this->CurDummyPaePos;
        }

        for($i = 0; $i < 10; ++$i)
        {
            $pae = $this->DummyPaesNum[$this->CurDummyPaePos];
            $this->Player2->HoldPaes[$pae] = $pae;
            ++$this->CurDummyPaePos;
        }

        for($i = 0; $i < 8; ++$i)
        {
            $pae = $this->DummyPaesNum[$this->CurDummyPaePos];
            $this->BadagPaes[$pae] = $pae;
            ++$this->CurDummyPaePos;
        }
        $this->BadagPaeCount = 8;

        $this->Player1->UserID = $p1UserID;
        $this->Player2->UserID = $p2UserID;

        $this->State = GameState::GAMMING;

        $this->SetPlayerTurn(true);
    }

    function GetNextTurnPlayerUserID() : string
    {
        return $this->GetPlayerData($this->GetNextTurnPlayerIndex())->UserID;
    }

    function GetNextTurnPlayerIndex() : int { return $this->CurTurnPlayerIndex == 0 ? 1:0;}

    function SetPlayerTurn(bool $isFirstTurn)
    {
        if($isFirstTurn == false)
        {
            $this->CurTurnPlayerIndex = 0;
            $this->WaitLimitTimeTurn = time() + self::TURN_WAIT_TIME + 3;
        }
        else
        {
            $this->CurTurnPlayerIndex = $this->GetNextTurnPlayerIndex();
            $this->WaitLimitTimeTurn = time() + self::TURN_WAIT_TIME;
        }

    }

    public function MakeGameStartNotifyPacket() : GameStartNtfJsoPacket
    {
        $packet = new GameStartNtfJsoPacket();
        $packet->TurnUserID = $this->GetCurTurnPlayerData()->UserID;
        $packet->P1PaeNums = $this->GetPlayerData(0)->HoldPaes;
        $packet->P2PaeNums = $this->GetPlayerData(1)->HoldPaes;
        $packet->BadagPaeNums = $this->BadagPaes;

        return $packet;
    }

    function TakeDummyPaes() : int
    {
        $pae = $this->DummyPaesNum[$this->CurDummyPaePos];
        ++$this->CurDummyPaePos;
        return $pae;
    }

    public function CheckPlayerTurnThrowPae(string $turnUserUD) : int
    {
        // 게임 중인가?
        if($this->IsGaming() == false)
        {
            return ErrorCode::GAME_PLAYER_TURN_INVALID_ROOM_STATE;
        }

        // 유저의 턴이 맞은가?
        if($this->IsTurnPlayer($turnUserUD) == false)
        {
            return ErrorCode::GAME_PLAYER_TURN_INVALID_TURN;
        }

        return ErrorCode::NONE;
    }

    //TODO 구현해야 한다
    public function AutoPlayerTurnThrowPae()
    {
        $throwPaeNum = -1;
        $selBadagPaeNum = -1;
        $this->ProcessPlayerTurnThrowPae($throwPaeNum, $selBadagPaeNum);
    }

    public function ProcessPlayerTurnThrowPae(int $throwPaeNum, int $selBadagPaeNum) : bool
    {
        $ret = $this->PlayerTurnThrowPae($throwPaeNum, $selBadagPaeNum);
        if($ret->ErrorCode != ErrorCode::NONE)
        {
            return false;
        }

        // 결과를 보낸다.
        $this->SendNotifyGamePlayerTurnThrowPae($ret, $this->GetNextTurnPlayerUserID());

        $turnRet = $this->PlayerTurnThrowPaeAfter($ret);

        if($turnRet->IsEndScore || $turnRet->IsEndEmptyPae)
        {
            $this->EndGame();
            $this->SendNotifyGameEnd($turnRet);
        }
        else
        {
            // 다음 턴 유저가 패가 하나도 없는 상태라면 바닥 패 하나를 까서 보낸다
            $this->IfPlayerEmptyHoldPaeThenGiveMorePae();
        }

        return true;
    }

    function PlayerTurnThrowPae(int $throwPaeNum, int $selBadagPaeNum) : PlayerThrowPaeResult
    {
        $retInfo = new PlayerThrowPaeResult();

        // 플레이어가 던진 패와 먹겠다는 바닥패가 문제 없는지 확인한다.
        $retInfo->ErrorCode = $this->PlayerTurnThrowPae_CheckThowPaeAndBadagPae($throwPaeNum, $selBadagPaeNum);

        if($retInfo->ErrorCode != ErrorCode::NONE)
        {
            return $retInfo;
        }

        // 더미 패에서 하나 개봉한다
        $retInfo->NewPaeNum = $this->DummyPaesNum[$this->CurDummyPaePos];
        ++$this->CurDummyPaePos;


        // 설사 아닌지 확인한다
        $retInfo->IsSeolsa = $this->PlayerTurnThrowPae_CheckSeolsa($throwPaeNum, $selBadagPaeNum, $retInfo->NewPaeNum);
        if ($retInfo->IsSeolsa)
        {
            return $retInfo;
        }

        $retInfo = $this->PlayerTurnThrowPae_TakePaes($throwPaeNum, $selBadagPaeNum, $retInfo->NewPaeNum);
        return $retInfo;
    }

    function PlayerTurnThrowPaeAfter(PlayerThrowPaeResult $throwResult) : TurnCheckResult
    {
        $result = new TurnCheckResult();

        $player = $this->GetCurTurnPlayerData();

        // 지금 먹은 패들을 지금까지 먹은 패에 추가한다.
        $this->AddPlayerOwnedPae($player, $throwResult);

        // 점수 계산
        $result->Score = $this->CheckScore($player);

        // go, stop 처리 등은 하지 않는다

        // 게임 종료?

        if($this->IsGameOverScore($result->Score))
        {
            $result->IsEndScore = true;
            $result->WinPlayer = $player->UserID;
            return $result;
        }

        if($this->IsGameOverEmptyDummyPae())
        {
            $result->IsEndEmptyPae = true;
            return $result;
        }

        // 다음턴 설정한다
        $this->SetPlayerTurn(false);
        return $result;
    }

    function AddPlayerOwnedPae(PlayerCurGameData &$player, PlayerThrowPaeResult $throwResult)
    {
        if($throwResult->IsSeolsa == false)
        {
            $player->AddOwnedPae($throwResult->TakePaeNumThrow);
            $player->AddOwnedPae($throwResult->TakePaeNumNew);
            $player->AddOwnedPae($throwResult->TakePaeNumBadag1);
            $player->AddOwnedPae($throwResult->TakePaeNumBadag2);
        }
        else
        {
            ++$player->SeolsaCount;
        }
    }

    // 간단하게 구현 하였다.
    function CheckScore(PlayerCurGameData &$player) : int
    {
        $score = 0;
        $score += $player->GwangScore();// 광 점수
        $score += $player->DanScore();// 단 점수
        $score += $player->TenScore();// 열피 점수
        $score += $player->PiScore();// 피 점수
        return $score;
    }

    function IsGameOverScore(int $curScore) : bool
    {
        // 점수가 일정 이상 이거나
        if($curScore >= 7)
        {
            return true;
        }

        // 바닥 패가 없으면 종료
        if(empty($this->DummyPaesNum))
        {
            return true;
        }

        return false;
    }

    function IsGameOverEmptyDummyPae() : bool
    {
        // 더미 패가 없으면 종료
        if(empty($this->DummyPaesNum))
        {
            return true;
        }

        return false;
    }



    function ShuffleDummyPaes()
    {
        // Fisher–Yates shuffle 알고리즘
        // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle

        for($i = HwatuInfo::MaxPaeCount - 1; $i >= 0; --$i)
        {
            $j = mt_rand($i);
            $this->DummyPaesNum[$i] = $this->DummyPaesNum[$j];
            $this->DummyPaesNum[$j] = $this->DummyPaesNum[$i];
	    }
    }

    function PlayerTurnThrowPae_CheckThowPaeAndBadagPae(int $throwPaeNum, int $selBadagPaeNum) : int
    {
        $retInfo = new PlayerThrowPaeResult();

        // 플레이어가 가지고 있는지 확인한다
        if($this->GetCurTurnPlayerData()->IsHaveHoldPae($throwPaeNum) == false)
        {
            return ErrorCode::GAME_PLAYER_TURN_INVALID_THROW_PAE_NUMBER;
        }

        // 던진 패로 바닥패에서 먹을 수 있는지 알아본다
        if($selBadagPaeNum != -1)
        {
            if (empty($this->BadagPaes[$selBadagPaeNum]))
            {
                return ErrorCode::GAME_PLAYER_TURN_INVALID_BADAG_PAE_NUMBER;
            }

            if (HwatuInfo::SameMonth($throwPaeNum, $this->BadagPaes[$selBadagPaeNum]))
            {
                return ErrorCode::GAME_PLAYER_TURN_NOT_SAME_THROW_BADAG_PAE_NUMBER;
            }
        }

        return ErrorCode::NONE;
    }

    function PlayerTurnThrowPae_CheckSeolsa(int $throwPaeNum, int $selBadagPaeNum, int $newPaeNum) : bool
    {
        $RetInfo = new PlayerThrowPaeResult();

        if (HwatuInfo::SameMonth($throwPaeNum, $selBadagPaeNum) &&
            HwatuInfo::SameMonth($throwPaeNum, $newPaeNum))
        {
            foreach($this->BadagPaes as $key => $value)
            {
                if($key != $selBadagPaeNum && HwatuInfo::SameMonth($throwPaeNum, $key))
                {
                    return false;
                }
            }

            $RetInfo->TakePaeNumThrow = &$throwPaeNum;
            $RetInfo->TakePaeNumBadag1 = $selBadagPaeNum;
            $RetInfo->TakePaeNumThrow = &$newPaeNum;
            $RetInfo->IsSeolsa = true;
            return true;
        }

        return false;
    }

    function PlayerTurnThrowPae_TakePaes(int $throwPaeNum, int $selBadagPaeNum, int $newPaeNum) : PlayerThrowPaeResult
    {
        $retInfo = new PlayerThrowPaeResult();
        $retInfo->NewPaeNum = $newPaeNum;

        // 개봉 패로 지금 던지 패를 먹거나 바닥 패를 먹을 수 있나?
        // 플레이어 패가 바닥 패를 먹었다면 개봉 패와 비교 불가
        if($selBadagPaeNum != -1 )
        {
            if(HwatuInfo::SameMonth($throwPaeNum, $retInfo->NewPaeNum))
            {
                $retInfo->TakePaeNumThrow = $throwPaeNum;
                $retInfo->TakePaeNumNew = $retInfo->NewPaeNum;
            }
            else
            {
                //바닥에 있는 패와 비교한다
                foreach($this->BadagPaes as $key => $value)
                {
                    if (HwatuInfo::SameMonth($retInfo->NewPaeNum, $key))
                    {
                        $retInfo->TakePaeNumBadag2 = $key;
                        $retInfo->TakePaeNumNew = $retInfo->NewPaeNum;
                        break;
                    }
                }
            }
        }
        else
        {
            $retInfo->TakePaeNumThrow = $throwPaeNum;
            $retInfo->TakePaeNumBadag1 = $this->BadagPaes[$selBadagPaeNum];
        }

        // 플레이어가 던진 패를 먹지 못했다면 바닥패에 추가한다
        if($retInfo->TakePaeNumThrow == -1)
        {
            $this->BadagPaes[$throwPaeNum] = $throwPaeNum;
        }
        else
        {
            unset($this->BadagPaes[$retInfo->TakePaeNumBadag1]);
        }

        // 개봉 패를 먹지 못했다면 바닥패에 추가한다
        if($retInfo->TakePaeNumBadag2 == -1)
        {
            $this->BadagPaes[$retInfo->NewPaeNum] = $retInfo->NewPaeNum;
        }
        else
        {
            unset($this->BadagPaes[$retInfo->TakePaeNumBadag2]);
        }

        return $retInfo;
    }


    function GetCurTurnPlayerData() : PlayerCurGameData
    {
        return $this->CurTurnPlayerIndex == 0 ? $this->Player1:$this->Player2;
    }

    function GetPlayerData(int $index) : PlayerCurGameData
    {
        return $index == 0 ? $this->Player1:$this->Player2;
    }

    function IfPlayerEmptyHoldPaeThenGiveMorePae()
    {
        $player = $this->GetCurTurnPlayerData();

        if(count($player->HoldPaes) > 0)
        {
            return;
        }

        $paeNum = $this->TakeDummyPaes();

        $player->HoldPaes[$paeNum] = $paeNum;

        $this->SendNotifyAddPaeTurnPlayer($paeNum);
    }


    function SendNotifyGamePlayerTurnThrowPae(PlayerThrowPaeResult $ret, string $nextTurnUserID)
    {
        $notify = new GamePlayerTurnThrowPaeNtfJsoPacket();
        $notify->TakePaeNumThrow = $ret->TakePaeNumThrow;
        $notify->TakePaeNumBadag1 = $ret->TakePaeNumBadag1;
        $notify->TakePaeNumNew = $ret->TakePaeNumNew;
        $notify->TakePaeNumBadag2 = $ret->TakePaeNumBadag2;
        $notify->NewPaeNum = $ret->NewPaeNum;
        $notify->IsSeolsa = $ret->IsSeolsa;
        $notify->NextTurnUserID = $nextTurnUserID;

        $json_data = json_encode($notify);
        $ntfPacket = PacketDesc::MakePacket(PacketID::NTF_GAME_PLAYER_TURN_THROW_PAE, $json_data);
        $packetLen = strlen($ntfPacket);
        $this->BroadcastSendPacketFunc[0](-1, $packetLen, $ntfPacket);
    }

    function SendNotifyAddPaeTurnPlayer(int $paeNum)
    {
        $bodyData = new GameAddPaeToTurnPlayerNtfJsonPacket();
        $bodyData->PaeNum = $paeNum;

        $json_data = json_encode($bodyData);
        $ntfPacket = PacketDesc::MakePacket(PacketID::NTF_GAME_ADD_PAE_TO_TURN_PLAYER, $json_data);
        $packetLen = strlen($ntfPacket);
        $this->BroadcastSendPacketFunc[0](-1, $packetLen, $ntfPacket);
    }

    function SendNotifyGameEnd(TurnCheckResult $turnRet)
    {
        $bodyData = new GameEndNtfJsonPacket();

        if($turnRet->IsEndEmptyPae == false)
        {
            $bodyData->Score = $turnRet->Score;
            $bodyData->WinPlayerUserID = $turnRet->WinPlayer;
        }
        else
        {
            $bodyData->IsDraw = true;
        }

        $json_data = json_encode($bodyData);
        $ntfPacket = PacketDesc::MakePacket(PacketID::NTF_GAME_END, $json_data);
        $packetLen = strlen($ntfPacket);
        $this->BroadcastSendPacketFunc[0](-1, $packetLen, $ntfPacket);
    }
}