using System.Collections.Generic;
/// <summary>
/// 모든 터미널 명령어 클래스가 구현해야 하는 인터페이스.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// 명령어를 실행하고 결과 문자열 리스트를 반환합니다.
    /// </summary>
    /// <param name="args">명령어 이름과 인자들을 포함하는 배열.</param>
    /// <returns>터미널에 출력될 결과 문자열의 리스트.</returns>
    List<string> Execute(string[] args);
}
