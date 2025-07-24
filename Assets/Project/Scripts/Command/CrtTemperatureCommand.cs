// 파일명: CrtTemperatureCommand.cs

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 'CRT_TEMPERATURE [INC/DEC]' 명령어를 처리하여 CRT의 온도를 조절합니다.
/// </summary>
public class CrtTemperatureCommand : ICommand
{
    public string Name => "CRT_TEMPERATURE";

    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();
        if (args.Length < 2)
        {
            lines.Add("SYSTEM > No parameter specified. Use INC or DEC.");
            return lines;
        }

        string param = args[1].ToUpper();
        if (param != "INC" && param != "DEC")
        {
            lines.Add("SYSTEM > Wrong parameter input.");
            return lines;
        }

        // GameManager의 온도 변수 직접 조절
        if (param == "INC")
        {
            GameManager.instance.CrtTemp += 1.5f;
            lines.Add("Thermal control UNIT operational... Rising to target temperature.");
        }
        else // DEC
        {
            GameManager.instance.CrtTemp -= 1.5f;
            lines.Add("Thermal control UNIT operational... Lowering to target temperature.");
        }

        // 상태 평가 및 색상 적용
        string status = GetTempStatus(GameManager.instance.CrtTemp);
        string coloredStatus = status switch
        {
            "STABILIZED" => "<color=#4D684E>[STABILIZED]</color>", // GREEN
            "OVERHEATED" => "<color=#ab1a1a>[OVERHEATED]</color>", // RED
            "UNDERCOOLED" => "<color=#3a67a6>[UNDERCOOLED]</color>", // BLUE
            _ => "[UNKNOWN]"
        };
        lines.Add($"Status: {coloredStatus}");
        return lines;
    }

    private string GetTempStatus(float temp)
    {
        if (temp >= 36.0f && temp <= 37.5f) return "STABILIZED";
        if (temp > 37.5f) return "OVERHEATED";
        return "UNDERCOOLED";
    }
}