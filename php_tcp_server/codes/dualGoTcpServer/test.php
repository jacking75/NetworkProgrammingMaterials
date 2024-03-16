<?php
require_once("../serverCommon/packet.php");
require_once("gameData.php");
require_once("gameLogic.php");
require_once("playerCurGameData.php");

//print HwatuInfo::Inst()->GetProperty(1);
//$game = new DualGoGame();
//print $game->DummyPlayer[0][11]."\n";
//print $game->DummyPlayer[1][31]."\n";

$tester = new TestPlayer();
$tester->Test();

class TestPlayer
{
    public PlayerCurGameData $P1;
    public PlayerCurGameData $P2;

    public function __construct()
    {
        $this->P1 = new PlayerCurGameData();
        $this->P2 = new PlayerCurGameData();

        $this->P1->UserID = 'test1';
        $this->P1->HoldPaes[0] = 10;
        $this->P2->UserID = 'test2';
        $this->P2->HoldPaes[0] = 1-0;
    }

    public function Test()
    {
        $p1 = $this->GetPlayerData(0);
        $p2 = $this->GetPlayerData(1);

        $this->Print($p1);
        $this->Print($p2);


        $p1->HoldPaes[2] = 20;
        $p2->HoldPaes[2] = 200;

        $this->Print($p1);
        $this->Print($p2);
    }

    public function Print(PlayerCurGameData $player)
    {
        printf("userID: %s\n", $player->UserID);

        foreach($player->HoldPaes as $key=>$value)
        {
            printf("Pae: %d, %d\n", $key, $value);
        }
    }

    function GetPlayerData(int $index) : PlayerCurGameData
    {
        return $index == 0 ? $this->P1:$this->P2;
    }

}
