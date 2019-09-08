using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.UI {
    internal class MainPresenter
    {
        private FormMain mView;
        private MainModel mModel = MainModel.Instance;
        public MainPresenter(FormMain view)
        {
            mView = view;
            init();
        }

        private void init()
        {
            mView.showStrategy(mModel.StrategyTable);
        }

        internal void loadPosition(string strategyName)
        {
            mView.showPosition(mModel.getPositionTable(strategyName));
        }

        internal void loadOrder(string strategyName)
        {
            mView.showOrder(mModel.getOrderTable(strategyName));
        }

        internal void setPostion(string strategyName, string instrumentID, int position)
        {
            mModel.setPosition(strategyName, instrumentID, position);
            loadPosition(strategyName);
        }

        internal void deleteStrategy(string strategyName)
        {
            mModel.deleteStrategy(strategyName);
            mView.showStrategy(mModel.StrategyTable);
        }
    }
}
