using System.Text;
using Newtonsoft.Json;

namespace Theseus.Core.Dto
{
    public class SignedObject
    {
        public byte[] Signature { get; set; }

        /// <summary>
        /// Public key of SignedObject sender.
        /// </summary>
        /// <value></value>
        public byte[] Key { get; set; }

        public string Base64Key()
        {
            return System.Convert.ToBase64String(Key);
        }

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