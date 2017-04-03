using System;

namespace TSS.UniverseLogic
{
    [Serializable]
	public class UniverseObject:IUniverseObject
	{
        protected int descriptor=0;
        protected int x =-1;
        protected int y=-1;

        public void SetCords(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public int GetX()
        {
            return x;
        }
        public int GetY()
        {
            return y;
        }
        public int GetDescriptor()
        {
            return descriptor;
        }
        public void Dispose()
        {
            descriptor = 0;
        }
        public bool isDisposed()
        {
            return (descriptor == 0);      
        }
	}
}
