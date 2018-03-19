using System;

/// <summary>
/// 経過時間を計測するためのクラス
/// </summary>
public sealed class SimpleStopWatch
{
	private DateTime mStartedDateTime;
	
	/// <summary>
	/// 計測された経過時間を返します
	/// </summary>
	public string ElapsedTime { get; private set; }
	
	/// <summary>
	/// 経過時間の計測を開始します
	/// </summary>
	public void Start()
	{
		mStartedDateTime = DateTime.Now;
	}
	
	/// <summary>
	/// 経過時間の計測を停止します
	/// </summary>
	public void Stop()
	{
		var ts = DateTime.Now - mStartedDateTime;
		ElapsedTime = string.Format(
			"{0:00}:{1:00}:{2:00}.{3:00}",
			ts.Hours, 
			ts.Minutes, 
			ts.Seconds,
			ts.Milliseconds / 10
			);
	}
}
