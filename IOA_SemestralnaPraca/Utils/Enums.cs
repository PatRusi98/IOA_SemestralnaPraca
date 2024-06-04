using System.ComponentModel;

namespace IOA_SemestralnaPraca.Utils
{
    public class Enums
    {
        public enum NodeType : short
        {
            [Description("Unspecified")]
            Unspecified = -1,
            [Description("Customer")]
            Customer = 0,
            [Description("Primary Source")]
            PrimarySource = 1,
            [Description("Possible Warehouse Location")]
            PossibleWarehouseLocation = 2
        }
    }
}