using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace exd.World.Helpers
{
    public class PromiseToken
    {
        private static long LastNewID = 0;
        private static long RequestNewID()
        {
            return Interlocked.Increment(ref LastNewID);
        }

        protected long ID;
        protected Action<PromiseToken> PromisedAction;
        public PromiseToken(Action<PromiseToken> promisedaction)
        {
            ID = RequestNewID();
            PromisedAction = promisedaction;
        }

        public void Done()
        {
            PromisedAction(this);
        }

        public override bool Equals(object obj)
        {
            var token = obj as PromiseToken;
            if (token == null)
                return false;

            return token.ID == ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}
