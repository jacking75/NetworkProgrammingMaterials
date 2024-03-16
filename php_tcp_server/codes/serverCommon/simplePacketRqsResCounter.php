<?php

class RqsResCounter
{
    public int $TimeSec;
    public int $ResCount;

    public function __construct()
    {
        $TimeSec = 0;
        $ResCount = 0;
    }
}

class SimplePacketRqsResCounter
{
    private static bool $IsEnable = false;
    private const MaxCount = 300;
    private static array $Counter;
    private static $CurIndex = -1;

    public static function Init()
    {
        self::$IsEnable = true;

        for($i = 0; $i < self::MaxCount; ++$i )
        {
            self::$Counter[$i] = new RqsResCounter();
        }

        print "[Enable] SimplePacketRqsResCounter\n";
    }

    public static function CountRes(int $curTimeSec, int $addCount)
    {
        if(self::$IsEnable == false)
        {
            return;
        }

        if(self::$CurIndex >= 0 && self::$Counter[self::$CurIndex]->TimeSec == $curTimeSec)
        {
            self::$Counter[self::$CurIndex]->ResCount += $addCount;
        }
        else {
            ++self::$CurIndex;

            if(self::$CurIndex >= self::MaxCount)
            {
                self::$CurIndex = 0;
            }

            self::$Counter[self::$CurIndex]->TimeSec = $curTimeSec;
            self::$Counter[self::$CurIndex]->ResCount = $addCount;
        }
    }

    public static function PrintLogFile()
    {
        if(self::$IsEnable == false)
        {
            return;
        }

        for($i = 0; $i < self::MaxCount; ++$i )
        {
            if(self::$Counter[$i]->TimeSec == 0 || self::$Counter[$i]->ResCount == 0)
            {
                continue;
            }

            $dt = date('m/d/Y H:i:s', self::$Counter[$i]->TimeSec);
            $msg = sprintf("[%s] ResCount: %d\n", $dt, self::$Counter[$i]->ResCount);
            error_log($msg,3,"./RqsResCounter.log");
        }
    }
}