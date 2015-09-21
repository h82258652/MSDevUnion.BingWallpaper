namespace AVOSCloud.RealtimeMessageV2
{
	public interface IAVIMMessage<T>
	where T : AVIMMessage
	{
		T Convert(AVIMMessage serverMessage);
	}
}