<?php

class ServerStatusView
{
    private const IntervalTimeSec = 3;

    private static int $CurTimeSec = 0;

    private static int $ConnectedCount = 0;
    private static int $ReqCountPerSec = 0;

    private static string $LogFileName = "";


    public static function Init(string $logFileName)
    {
        self::$LogFileName = $logFileName;
    }

    public static function Update()
    {
        $cur = time();
        if($cur >= (self::$CurTimeSec+self::IntervalTimeSec))
        {
            $usageMB = memory_get_usage() / (1024 * 1024);

            $msg = sprintf("[Status][%s][pid:%d] memoryUsage:%d MB, CPU:%d, ConnCnt:%d, ReqCount/3s:%d\n",
                date("Y-m-d H:i:s"), getmypid(), $usageMB, self::GetCPUUsage(), self::$ConnectedCount, self::$ReqCountPerSec);

            print $msg;

            if(self::$LogFileName != "")
            {
                error_log($msg, 3, self::$LogFileName);
            }

            self::$ReqCountPerSec = 0;
            self::$CurTimeSec = $cur;
        }
    }

    public static function IncrementConnected()
    {
        ++self::$ConnectedCount;
    }

    public static function DecrementConnected()
    {
        --self::$ConnectedCount;
    }

    public static function IncrementReq()
    {
        ++self::$ReqCountPerSec;
    }


    static function GetCPUUsage()
    {
        if(strtoupper(substr(PHP_OS, 0, 3)) === 'WIN')
        {
            return -1;
        }

        $load = sys_getloadavg();
        return $load[0];
    }
}