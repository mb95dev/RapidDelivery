﻿namespace Orders.Application.DTO;

public record PaymentDto(string CardName, string CardNumber, string Expiration, string Cvv, int PaymentMethod);
