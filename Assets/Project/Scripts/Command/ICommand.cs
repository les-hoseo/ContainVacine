// 파일명: ICommand.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 터미널 명령어 클래스가 구현해야 하는 인터페이스입니다.
/// </summary>
public interface ICommand
{
    string Name { get; }
    List<string> Execute(string[] args);


}
