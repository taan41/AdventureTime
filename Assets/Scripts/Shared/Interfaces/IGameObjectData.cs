public interface IGameObjectData
{
	void CopyFrom(IGameObjectData other);
	bool IsValid() => true;
}