<?php
require_once ("gameData.php");

class PlayerCurGameData
{
    public string $UserID;

    public array $HoldPaes;

    public array $GwangPaes;
    public array $DanPaes;
    public array $TenPaes;
    public array $PiPaes;

    public int $SeolsaCount;

    public function Clear()
    {
        $this->HoldPaes = array();
        $this->GwangPaes = array();
        $this->DanPaes = array();
        $this->TenPaes = array();
        $this->PiPaes = array();
        $this->SeolsaCount = 0;
    }

    public function IsHaveHoldPae(int $paeNum) : bool
    {
        return !empty($this->HoldPaes[$paeNum]);
    }

    public function AddOwnedPae(int $paeNum)
    {
        if($paeNum < 0)
        {
            return;
        }

        if(HwatuInfo::Inst()->IsGwangPae($paeNum))
        {
            $this->GwangPaes[$paeNum] = $paeNum;
        }
        else if(HwatuInfo::Inst()->IsDanPae($paeNum))
        {
            $this->DanPaes[$paeNum] = $paeNum;
        }
        else if(HwatuInfo::Inst()->IsTenPae($paeNum))
        {
            $this->TenPaes[$paeNum] = $paeNum;
        }
        else
        {
            $this->PiPaes[$paeNum] = $paeNum;
        }
    }

    public function GwangScore() : int
    {
        return count($this->GwangPaes);
    }

    public function DanScore() : int
    {
        $count = count($this->DanPaes);
        if($count < 5)
        {
            return 0;
        }

        return 1 + ($count - 5);
    }

    public function TenScore() : int
    {
        $count = count($this->TenPaes);
        if($count < 5)
        {
            return 0;
        }

        return 1 + ($count - 5);
    }

    public function PiScore() : int
    {
        $count = count($this->PiPaes);
        if($count < 10)
        {
            return 0;
        }

        return 1 + ($count - 10);
    }
}