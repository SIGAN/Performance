namespace Performance.Monitoring
{
	public interface IMonitor
	{
		void Check();
		void Start();
		void StartTimer();
		void Stop();
		void StopTimer();
	}
}