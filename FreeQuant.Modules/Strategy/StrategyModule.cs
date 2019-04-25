using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FreeQuant.Modules {
    internal class StrategyModule {
        private static StrategyModule mInstance = new StrategyModule();

        public static StrategyModule Instance => mInstance;

        private StrategyModule() {
            loadStrategy();
        }

        //策略添加
        private Dictionary<string, BaseStrategy> mStrategyMap = new Dictionary<string, BaseStrategy>();
        private void loadStrategy() {
            //获取文件列表 
            string[] files = new string[] { };
            try {
                files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\strategys");
            } catch (Exception ex) {
                FqLog.EnginLog(ex.StackTrace);
            }

            //加载策略
            foreach (string f in files) {
                Assembly assembly = Assembly.LoadFrom(f);
                Type[] types = assembly.GetTypes();
                foreach (Type t in types) {
                    if (!t.IsSubclassOf(typeof(BaseStrategy)))
                        continue;

                    BaseStrategy stg = Activator.CreateInstance(t) as BaseStrategy;
                    if (stg == null)
                        continue;

                    addStrategy(t.Name, stg);
                }
            }
        }
        internal void addStrategy(string name, BaseStrategy stg) {
            //策略去重
            if (mStrategyMap.ContainsKey(name))
                return;
            mStrategyMap.Add(name, stg);

            //行情
            stg.OnSubscribeInstrument += Subscribe;

            //交易
            stg.OnSendOrder += sendOrder;
            stg.OnCancelOrder += cancelOrder;
            stg.OnChangePosition += changePosition;
        }

        #region 策略启停

        private void startStrategy() {
            foreach (BaseStrategy stg in mStrategyMap.Values) {
                stg.SendStart();
            }
        }

        private void stopStrategy() {
            foreach (BaseStrategy stg in mStrategyMap.Values) {
                stg.SendStop();
            }
        }

        #endregion

        #region 处理行情
        //获取行情合约
        private TickDispatcher mTickDispatcher = new TickDispatcher();
        internal List<Instrument> InstrumentList => mTickDispatcher.InstrumentList;
        private void Subscribe(BaseStrategy stg, Instrument inst) {
            mTickDispatcher.SubscribTick(inst, stg);
        }
        #endregion

        #region 处理交易
        private void sendOrder(Order order) {

        }

        private void cancelOrder(Order order) {

        }

        private void changePosition(Position position) {

        }
        #endregion

    }
}
