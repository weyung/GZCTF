﻿using CTFServer.Hubs;
using CTFServer.Hubs.Clients;
using CTFServer.Models.Request.Edit;
using CTFServer.Repositories.Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog.Data;

namespace CTFServer.Repositories;

public class GameNoticeRepository : RepositoryBase, IGameNoticeRepository
{
    private readonly IMemoryCache cache;
    private IHubContext<UserHub, IUserClient> hubContext;

    public GameNoticeRepository(IMemoryCache memoryCache,
        IHubContext<UserHub, IUserClient> hub,
        AppDbContext _context) : base(_context)
    {
        cache = memoryCache;
        hubContext = hub;
    }

    public async Task<GameNotice> AddNotice(GameNotice notice, CancellationToken token = default)
    {
        await context.AddAsync(notice, token);
        await SaveAsync(token);

        cache.Remove(CacheKey.GameNotice(notice.GameId));

        await hubContext.Clients.Group($"Game_{notice.GameId}")
            .ReceivedGameNotice(notice);

        return notice;
    }

    public Task<GameNotice[]> GetNormalNotices(int gameId, CancellationToken token = default)
        => context.GameNotices
            .Where(n => n.GameId == gameId && n.Type == NoticeType.Normal)
            .ToArrayAsync(token);

    public Task<GameNotice?> GetNoticeById(int gameId, int noticeId, CancellationToken token = default)
        => context.GameNotices.FirstOrDefaultAsync(e => e.Id == noticeId && e.GameId == gameId, token);

    public Task<GameNotice[]> GetNotices(int gameId, int count = 10, int skip = 0, CancellationToken token = default)
        => cache.GetOrCreateAsync(CacheKey.GameNotice(gameId), (entry) =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            return context.GameNotices.Where(e => e.GameId == gameId)
                .OrderByDescending(e => e.PublishTimeUTC)
                .Skip(skip).Take(count)
                .ToArrayAsync(token);
        });

    public Task RemoveNotice(GameNotice notice, CancellationToken token = default)
    {
        context.Remove(notice);
        cache.Remove(CacheKey.GameNotice(notice.GameId));

        return SaveAsync(token);
    }

    public async Task<GameNotice> UpdateNotice(GameNotice notice, CancellationToken token = default)
    {
        notice.PublishTimeUTC = DateTimeOffset.UtcNow;
        await SaveAsync(token);

        cache.Remove(CacheKey.GameNotice(notice.GameId));

        return notice;
    }
}