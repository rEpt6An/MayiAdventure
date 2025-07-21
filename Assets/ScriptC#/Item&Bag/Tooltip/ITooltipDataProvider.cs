// ITooltipDataProvider.cs

// 这是一个接口，它不是一个类，只是一个“行为规范”
public interface ITooltipDataProvider
{
    string GetTooltipHeader();
    string GetTooltipContent();
}