using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using System.Collections.Generic;

public class RebateService : IRebateService
{
    private readonly IRebateDataStore _rebateDataStore;
    private readonly IProductDataStore _productDataStore;
    private readonly IDictionary<IncentiveType, IRebateCalculator> _calculators;

    public RebateService(IRebateDataStore rebateDataStore, IProductDataStore productDataStore)
    {
        _rebateDataStore = rebateDataStore;
        _productDataStore = productDataStore;
        _calculators = new Dictionary<IncentiveType, IRebateCalculator>
        {
            { IncentiveType.AmountPerUom, new AmountPerUomCalculator() },
            { IncentiveType.FixedCashAmount, new FixedCashAmountCalculator() },
            { IncentiveType.FixedRateRebate, new FixedRateRebateCalculator() }
        };
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        CalculateRebateResult result = new CalculateRebateResult();

        Rebate rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
        Product product = _productDataStore.GetProduct(request.ProductIdentifier);

        if (rebate != null && product != null && _calculators.TryGetValue(rebate.Incentive, out var calculator))
        {
            // Calulate and store
            result = calculator.Calculate(rebate, product, request.Volume);
            if (result.Success)
            {
                _rebateDataStore.StoreCalculationResult(rebate, result.RebateAmount);
            }
        }     

        return result;
    }
}