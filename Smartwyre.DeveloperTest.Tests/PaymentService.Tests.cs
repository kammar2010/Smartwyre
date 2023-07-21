using Moq;
using Xunit;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Tests;

public class PaymentServiceTests
{
    [Theory]
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.FixedRateRebate, 8)]
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.AmountPerUom, 4)]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedCashAmount, 2)]
    public void RebateSuccess(IncentiveType rebateIncentive, SupportedIncentiveType productIncentive, decimal expected)
    {
        // Setup
        var mockedRebateDataStore = new Mock<IRebateDataStore>();
        mockedRebateDataStore.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(CreateRebateMock(rebateIncentive, 2, 2));
        var mockedProductDataStore = new Mock<IProductDataStore>();
        mockedProductDataStore.Setup(p => p.GetProduct(It.IsAny<string>())).Returns(CreateProductMock(productIncentive, 2));

        // Execute
        var rebateService = new RebateService(mockedRebateDataStore.Object, mockedProductDataStore.Object);
        CalculateRebateResult result = rebateService.Calculate(new CalculateRebateRequest() { Volume = 2 });

        // Validate
        mockedRebateDataStore.Verify(m => m.StoreCalculationResult(It.IsAny<Rebate>(), result.RebateAmount), Times.Once);
        Assert.True(result.Success);        
        Assert.Equal(expected, result.RebateAmount, 2);
    }

    [Theory]
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.AmountPerUom)]
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.FixedCashAmount)]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedRateRebate)]
    public void RebateTypeMismatch(IncentiveType rebateIncentive, SupportedIncentiveType productIncentive)
    {
        // Setup
        var mockedRebateDataStore = new Mock<IRebateDataStore>();
        mockedRebateDataStore.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(CreateRebateMock(rebateIncentive, 2, 2));
        var mockedProductDataStore = new Mock<IProductDataStore>();
        mockedProductDataStore.Setup(p => p.GetProduct(It.IsAny<string>())).Returns(CreateProductMock(productIncentive, 2));

        // Execute
        var rebateService = new RebateService(mockedRebateDataStore.Object, mockedProductDataStore.Object);
        CalculateRebateResult result = rebateService.Calculate(new CalculateRebateRequest() { Volume = 2 });

        // Validate
        Assert.False(result.Success);
        Assert.Equal(0, result.RebateAmount, 2);
    }


    [Theory]
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.FixedRateRebate, 0, 0, 2, 0)]
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.FixedRateRebate, 2, 0, 2, 0)]
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.FixedRateRebate, 2, 2, 2, 0)]
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.AmountPerUom, 2, 0, 0, 2)]
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.AmountPerUom, 2, 2, 0, 2)]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedCashAmount, 2, 2, 0, 2)]
    public void RebateZero(IncentiveType rebateIncentive, SupportedIncentiveType productIncentive, decimal price, decimal volume, decimal amount, decimal percent)
    {
        // Setup
        var mockedRebateDataStore = new Mock<IRebateDataStore>();
        mockedRebateDataStore.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(CreateRebateMock(rebateIncentive, amount, percent));
        var mockedProductDataStore = new Mock<IProductDataStore>();
        mockedProductDataStore.Setup(p => p.GetProduct(It.IsAny<string>())).Returns(CreateProductMock(productIncentive, price));

        // Execute
        var rebateService = new RebateService(mockedRebateDataStore.Object, mockedProductDataStore.Object);
        CalculateRebateResult result = rebateService.Calculate(new CalculateRebateRequest() { Volume = volume });

        // Validate
        Assert.False(result.Success);
        Assert.Equal(0, result.RebateAmount, 2);
    }


    private Product CreateProductMock(SupportedIncentiveType incentive, decimal price)
    {
        return new Product { Price = price, SupportedIncentives = incentive };
    }

    private Rebate CreateRebateMock(IncentiveType incentive, decimal amount, decimal percent)
    {
        return new Rebate { Amount = amount, Incentive = incentive, Percentage = percent };
    }
}
