using System.ComponentModel;
using System.Reflection;

namespace Dealytics.Domain.Enums
{
    public enum GameSituation
    {
        None,
        [Description("OpenRaise")]
        OpenRaise,
        [Description("BigBlindVsSmallBlind")]
        BigBlindVsSmallBlind,
        [Description("RaiseOverLimpers")]
        RaiseOverLimpers,
        [Description("ColdFourBet")]
        Cold4Bet,
        [Description("FourBet")]
        FourBet,
        [Description("Squeeze")]
        Squeeze,
        [Description("ThreeBet")]
        ThreeBet,
        [Description("VsSqueeze")]
        VsSqueeze,
        [Description("VsThreeBet")]
        VsThreeBet,
        [Description("VsThreeBetAndCall")]
        VsThreeBetAndCall
    }

    // Método para obtener la descripción
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo? field = value.GetType().GetField(value.ToString());
            DescriptionAttribute? attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }
    }

}

