using System.Collections;
namespace Cosmos.CosmosEditor
{
    public class EditorCoroutine
    {
        public EditorCoroutineCore.ICoroutineYield CurrentYield;
        public IEnumerator Routine;
        public string RoutineUniqueHash;
        public string OwnerUniqueHash;
        public string MethodName = "";

        public int OwnerHash;
        public string OwnerType;
        public bool Finished = false;
        public EditorCoroutine(IEnumerator routine, int ownerHash, string ownerType)
        {
            this.Routine = routine;
            this.OwnerHash = ownerHash;
            this.OwnerType = ownerType;
            OwnerUniqueHash = ownerHash + "_" + ownerType;
            if (routine != null)
            {
                string[] split = routine.ToString().Split('<', '>');
                if (split.Length == 3)
                {
                    this.MethodName = split[1];
                }
            }
            RoutineUniqueHash = ownerHash + "_" + ownerType + "_" + MethodName;
        }
        public EditorCoroutine(string methodName, int ownerHash, string ownerType)
        {
            this.MethodName = methodName;
            this.OwnerHash = ownerHash;
            this.OwnerType = ownerType;
            OwnerUniqueHash = ownerHash + "_" + ownerType;
            RoutineUniqueHash = ownerHash + "_" + ownerType + "_" + methodName;
        }
    }
}
