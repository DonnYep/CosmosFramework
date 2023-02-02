namespace Cosmos.Procedure
{
    /// <summary>
    /// 流程节点基类；
    /// </summary>
    public abstract class ProcedureState : ProcedureFsmState<IProcedureManager>
    {
        /*
         * 1、流程节点的生命周期按照顺序依次为:OnInit>OnEnter>OnUpdate>OnExit>OnDestroy;
         * 
         * 2、OnInit函数在ProcedureNode被添加到ProcedureManager时触发。
         * 
         * 3、OnEnter函数在进入ProcedureNode状态时触发。
         * 
         * 4、OnUpdate函数在ProcedureNode状态中轮询触发。
         * 
         * 5、OnExit函数在离开ProcedureNode状态时触发。
         * 
         * 6、OnDestroy函数在ProcedureNode被从ProcedureManager移除时触发。
         * 
         */
    }
}
