namespace BrewUp.Shared.Messages.Requests;

public class WhatIfInventoryImpact(string beerName,
    decimal beerQuantity,
    string originalQuestions,
    Guid correlationId) : AgentRequest(correlationId)
{
    public string BeerName { get; private set; } = beerName;
    public decimal BeerQuantity { get; private set; } = beerQuantity;
    public string OriginalQuestions { get; private set; } = originalQuestions;
}