using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using exd.World.Resources;

namespace exd.World.Helpers
{
    public class PromiseToken
    {
        private static long LastNewID = 0;
        private static long RequestNewID()
        {
            return Interlocked.Increment(ref LastNewID);
        }

        private static List<PromiseToken> PromisedTokens = new List<PromiseToken>();

        protected long ID;
        protected Action<PromiseToken> PromisedAction;
        protected AbstractGroundResource PromisedResource;
        public PromiseToken(Action<PromiseToken> promisedaction, AbstractGroundResource promisedresource)
        {
            ID = RequestNewID();
            PromisedAction = promisedaction;
            PromisedResource = promisedresource;

            PromisedTokens.Add(this);
        }

        public void Done()
        {
            PromisedAction(this);
            PromisedTokens.Remove(this);
        }

        public static PromiseToken GetTokenByResourcePromised(AbstractGroundResource res)
        {
            return PromisedTokens.FirstOrDefault(t => t.PromisedResource == res);
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
