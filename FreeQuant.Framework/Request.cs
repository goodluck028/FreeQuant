using System;

namespace FreeQuant.Framework {
    public class Request<T, M>:IRequest {
        private T mRequestBody;
        private Action<M> mResponseAction;
        private Action<Exception> mErrorAction;
        private bool handled = false;

        public Request(T request) {
            mRequestBody = request;
        }

        public Request<T, M> SetResponseAction(Action<M> act) {
            mResponseAction = act;
            return this;
        }

        public Request<T, M> SetErrorAction(Action<Exception> act) {
            mErrorAction = act;
            return this;
        }

        public void Send() {
            EventBus.SendRequest(this);
        }

        public T RequestBody => mRequestBody;

        public void Response(M m) {
            if (!handled) {
                mResponseAction?.Invoke(m);
            }

            handled = true;
        }

        public void Error(Exception ex) {
            if (!handled) {
                mErrorAction?.Invoke(ex);
            }

            handled = true;
        }
    }

    public interface IRequest
    {
        void Error(Exception ex);
    }
}
