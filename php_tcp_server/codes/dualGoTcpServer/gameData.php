<?php
require_once("errorCode.php");

abstract class HwatuPaeProperty
{
    const NONE = 0;
    const GWANG = 1;
    const HONGDAN = 5;
    const CHEONGDAN = 6;
    const CHODAN = 7;

    const BIRD = 9;
    const TEN = 10;
}

class PlayerThrowPaeResult
{
    public int $ErrorCode = ErrorCode::NONE;

    // 먹은 패들
    public int $TakePaeNumThrow = -1; //플레이어가 던진 패
    public int $TakePaeNumBadag1 = -1; //던진 패로 먹은 바닥 패
    public int $TakePaeNumNew = -1; //새로 받은 패
    public int $TakePaeNumBadag2 = -1;//새로 받은 패로 먹은 바닥 패

    public int $NewPaeNum = -1;
    public bool $IsSeolsa = false;
}

class TurnCheckResult
{
    public bool $IsEndScore = false;
    public bool $IsEndEmptyPae = false;
    public int $Score = 0;
    public string $WinPlayer;
}


class HwatuPae
{
    public int $Index;
    public int $Property;

    public function __construct(int $index, int $property)
    {
        $this->Index = $index;
        $this->Property = $property;
    }
}

class HwatuInfo
{
    public const MaxPaeCount = 48;
    private static $instance;
    private $Paes = [];

    public static function Inst()
    {
        if (null === self::$instance) {
            self::$instance = new HwatuInfo();

            // 1월
            self::$instance->Paes[0] = new HwatuPae(0, HwatuPaeProperty::GWANG);
            self::$instance->Paes[1] = new HwatuPae(1, HwatuPaeProperty::HONGDAN);
            self::$instance->Paes[2] = new HwatuPae(2, HwatuPaeProperty::NONE);
            self::$instance->Paes[3] = new HwatuPae(3, HwatuPaeProperty::NONE);

            // 2월
            self::$instance->Paes[4] = new HwatuPae(4, HwatuPaeProperty::BIRD);
            self::$instance->Paes[5] = new HwatuPae(5, HwatuPaeProperty::HONGDAN);
            self::$instance->Paes[6] = new HwatuPae(6, HwatuPaeProperty::NONE);
            self::$instance->Paes[7] = new HwatuPae(7, HwatuPaeProperty::NONE);

            // 3월
            self::$instance->Paes[8] = new HwatuPae(8, HwatuPaeProperty::GWANG);
            self::$instance->Paes[9] = new HwatuPae(9, HwatuPaeProperty::HONGDAN);
            self::$instance->Paes[10] = new HwatuPae(10, HwatuPaeProperty::NONE);
            self::$instance->Paes[11] = new HwatuPae(11, HwatuPaeProperty::NONE);

            // 4월
            self::$instance->Paes[12] = new HwatuPae(12, HwatuPaeProperty::BIRD);
            self::$instance->Paes[13] = new HwatuPae(13, HwatuPaeProperty::CHODAN);
            self::$instance->Paes[14] = new HwatuPae(14, HwatuPaeProperty::NONE);
            self::$instance->Paes[15] = new HwatuPae(15, HwatuPaeProperty::NONE);

            // 5월
            self::$instance->Paes[16] = new HwatuPae(16, HwatuPaeProperty::TEN);
            self::$instance->Paes[17] = new HwatuPae(17, HwatuPaeProperty::CHODAN);
            self::$instance->Paes[18] = new HwatuPae(18, HwatuPaeProperty::NONE);
            self::$instance->Paes[19] = new HwatuPae(19, HwatuPaeProperty::NONE);

            // 6월
            self::$instance->Paes[20] = new HwatuPae(20, HwatuPaeProperty::TEN);
            self::$instance->Paes[21] = new HwatuPae(21, HwatuPaeProperty::CHEONGDAN);
            self::$instance->Paes[22] = new HwatuPae(22, HwatuPaeProperty::NONE);
            self::$instance->Paes[23] = new HwatuPae(23, HwatuPaeProperty::NONE);

            // 7월
            self::$instance->Paes[24] = new HwatuPae(24, HwatuPaeProperty::TEN);
            self::$instance->Paes[25] = new HwatuPae(25, HwatuPaeProperty::CHODAN);
            self::$instance->Paes[26] = new HwatuPae(26, HwatuPaeProperty::NONE);
            self::$instance->Paes[27] = new HwatuPae(27, HwatuPaeProperty::NONE);

            // 8월
            self::$instance->Paes[28] = new HwatuPae(28, HwatuPaeProperty::GWANG);
            self::$instance->Paes[29] = new HwatuPae(29, HwatuPaeProperty::BIRD);
            self::$instance->Paes[30] = new HwatuPae(30, HwatuPaeProperty::NONE);
            self::$instance->Paes[31] = new HwatuPae(31, HwatuPaeProperty::NONE);

            // 9월
            self::$instance->Paes[32] = new HwatuPae(32, HwatuPaeProperty::TEN);
            self::$instance->Paes[33] = new HwatuPae(33, HwatuPaeProperty::CHEONGDAN);
            self::$instance->Paes[34] = new HwatuPae(34, HwatuPaeProperty::NONE);
            self::$instance->Paes[35] = new HwatuPae(35, HwatuPaeProperty::NONE);

            // 10월
            self::$instance->Paes[36] = new HwatuPae(36, HwatuPaeProperty::TEN);
            self::$instance->Paes[37] = new HwatuPae(37, HwatuPaeProperty::CHEONGDAN);
            self::$instance->Paes[38] = new HwatuPae(38, HwatuPaeProperty::NONE);
            self::$instance->Paes[39] = new HwatuPae(39, HwatuPaeProperty::NONE);

            // 11월
            self::$instance->Paes[40] = new HwatuPae(40, HwatuPaeProperty::GWANG);
            self::$instance->Paes[41] = new HwatuPae(41, HwatuPaeProperty::TEN);
            self::$instance->Paes[42] = new HwatuPae(42, HwatuPaeProperty::NONE);
            self::$instance->Paes[43] = new HwatuPae(43, HwatuPaeProperty::NONE);

            // 12월
            self::$instance->Paes[44] = new HwatuPae(44, HwatuPaeProperty::GWANG);
            self::$instance->Paes[45] = new HwatuPae(45, HwatuPaeProperty::BIRD);
            self::$instance->Paes[46] = new HwatuPae(46, HwatuPaeProperty::CHODAN);
            self::$instance->Paes[47] = new HwatuPae(47, HwatuPaeProperty::TEN);
        }

        return self::$instance;
    }

    static public function SameMonth(int $paeNum1, int $paeNum2) : bool
    {
        if(($paeNum1/4) != ($paeNum2/4))
        {
            return fasle;
        }

        return true;
    }

    public function IsGwangPae(int $paeNum) : bool
    {
        return $this->Paes[$paeNum]->Property == HwatuPaeProperty::GWANG;
    }

    public function IsDanPae(int $paeNum) : bool
    {
        if($this->IsHongDanPae($paeNum) || $this->IsCheongDanPae($paeNum) ||
        $this->IsChoDanPae($paeNum))
        {
            return true;
        }

        return false;
    }
    public function IsHongDanPae(int $paeNum) : bool
    {
        return $this->Paes[$paeNum]->Property == HwatuPaeProperty::HONGDAN;
    }
    public function IsCheongDanPae(int $paeNum) : bool
    {
        return $this->Paes[$paeNum]->Property == HwatuPaeProperty::CHEONGDAN;
    }
    public function IsChoDanPae(int $paeNum) : bool
    {
        return $this->Paes[$paeNum]->Property == HwatuPaeProperty::CHODAN;
    }

    public function IsTenPae(int $paeNum) : bool
    {
        if($this->Paes[$paeNum]->Property == HwatuPaeProperty::TEN ||
            $this->Paes[$paeNum]->Property == HwatuPaeProperty::BIRD)
        {
            return true;
        }
        return false;
    }

    public function IsBirdPae(int $paeNum) : bool
    {
        return $this->Paes[$paeNum]->Property == HwatuPaeProperty::BIRD;
    }

    public function IsPiPae(int $paeNum) : bool
    {
        return $this->Paes[$paeNum]->Property == HwatuPaeProperty::NONE;
    }


//    function GetProperty(int $paeNum) : int
//    {
//        return $this->Paes[$paeNum]->Property;
//    }

    static public function EnableTwoPee(int $paeNum) : bool
    {
        if($paeNum == 32 || $paeNum == 41 || $paeNum == 47)
        {
            return true;
        }

        return false;
    }

    private function __construct()
    {
    }

    private function __clone()
    {
    }
}
