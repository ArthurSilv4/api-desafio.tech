﻿namespace api_desafio.tech.DTOs
{
    public record ChallengeDto(Guid Id, string? Title, string? Description, DateTime StartDate, DateTime EndDate, List<DateTime> ChallengeDates, bool Completed, Guid? UserId);
}
