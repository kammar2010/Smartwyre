using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services
{
    public interface IRebateCalculator
    {
        CalculateRebateResult Calculate(Rebate rebate, Product product, decimal volume);
    }
}
