using System.Text;
using Newtonsoft.Json;

namespace Theseus.Core.Dto
{
    public class SignedObject
    {
        public byte[] Signature { get; set; }
        public byte[] Key { get; set; }

        public byte[] PlainData()
        {
            var cloneThis = (SignedObject)this.MemberwiseClone();
            cloneThis.Signature = null;
            cloneThis.Key = null;

            var json = JsonConvert.SerializeObject(cloneThis, Formatting.None);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}