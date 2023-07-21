using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services
{
    public class FixedCashAmountCalculator : IRebateCalculator
    {
        public CalculateRebateResult Calculate(Rebate rebate, Product product, decimal volume)
        {
            var result = new CalculateRebateResult();

            if (product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount))
            {
                result.RebateAmount = rebate.Amount;
                result.Success = result.RebateAmount != 0;
            }

            return result;
        }
    }

    public class FixedRateRebateCalculator : IRebateCalculator
    {
        public CalculateRebateResult Calculate(Rebate rebate, Product product, decimal volume)
        {
            var result = new CalculateRebateResult();

            if (product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate))
            {
                result.RebateAmount = product.Price * rebate.Percentage * volume;
                result.Success = result.RebateAmount != 0;
            }

            return result;
        }
    }

    public class AmountPerUomCalculator : IRebateCalculator
    {
        public CalculateRebateResult Calculate(Rebate rebate, Product product, decimal volume)
        {
            var result = new CalculateRebateResult();

            if (product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom))
            {
                result.RebateAmount = rebate.Amount * volume;
                result.Success = result.RebateAmount != 0;
            }

            return result;
        }
    }
}