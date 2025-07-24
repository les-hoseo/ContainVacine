using System.Collections.Generic;
using System.Diagnostics;
using Unity;

public class TestDmgCommand : ICommand
{
    public string Name => "Dmg";

    public List<string> Execute(string[] args)
    {
        // 1. Instance가 null인지 확인
        if (GimmickManager.Instance != null)
        {
            GimmickManager.Instance.TriggerGimmick("DMAGED");
        }
        else
        {
            Debug.WriteLine("GimmickManager 인스턴스가 존재하지 않음!");
        }

        return new List<string>() { "dmg" };
    }
}
