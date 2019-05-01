using Imml;
using Imml.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMLSharp.Services
{
    public class InputService
    {
        private Dictionary<EventType, List<Trigger>> _Triggers;

        public InputService()
        {
            _Triggers = new Dictionary<EventType, List<Trigger>>();
        }

        public void Register(Trigger trigger)
        {
            
            
        }


    }
}
