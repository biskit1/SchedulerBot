﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using SchedulerBot.Data.Models;

namespace SchedulerBot.Data.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly SchedulerBotContext _db;

        public CalendarService(SchedulerBotContext context) => _db = context;

        public async Task<Calendar> CreateCalendarAsync(Calendar calendar)
        {
            await _db.Calendars.AddAsync(calendar);
            await _db.SaveChangesAsync();

            return calendar;
        }

        public async Task<bool> DeleteCalendarAsync(ulong calendarId)
        {
            var permissionsToRemove = await _db.Permissions.Where(p => p.Calendar.Id == calendarId).ToListAsync();
            var calendarToRemove = await _db.Calendars.FirstOrDefaultAsync(c => c.Id == calendarId);

            _db.Permissions.RemoveRange(permissionsToRemove);
            _db.Calendars.Remove(calendarToRemove);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<int> ResolveCalendarPrefixAsync(ulong calendarId, string message)
        {
            var prefix = await _db.Calendars
                .Where(c => c.Id == calendarId)
                .Select(c => c.Prefix)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(prefix) || !message.StartsWith(prefix))
            {
                return -1;
            }

            return prefix.Length;
        }

        public async Task<string> GetCalendarPrefixAsync(ulong calendarId)
        {
            var prefix = await _db.Calendars
                .Where(c => c.Id == calendarId)
                .Select(c => c.Prefix)
                .FirstOrDefaultAsync();

            return prefix;
        }

        public async Task<string> UpdateCalendarPrefixAsync(ulong calendarId, string newPrefix)
        {
            var calendar = await _db.Calendars.FirstOrDefaultAsync(c => c.Id == calendarId);
            calendar.Prefix = newPrefix;

            await _db.SaveChangesAsync();
            return calendar.Prefix;
        }

        public async Task<ulong> GetCalendarDefaultChannelAsync(ulong calendarId)
        {
            var defaultChannel = await _db.Calendars
                .Where(c => c.Id == calendarId)
                .Select(c => c.DefaultChannel)
                .FirstOrDefaultAsync();

            return defaultChannel;
        }

        public async Task<ulong> UpdateCalendarDefaultChannelAsync(ulong calendarId, ulong newDefaultChannel)
        {
            var calendar = await _db.Calendars.FirstOrDefaultAsync(c => c.Id == calendarId);
            calendar.DefaultChannel = newDefaultChannel;

            await _db.SaveChangesAsync();
            return calendar.DefaultChannel;
        }

        public async Task<string> GetCalendarTimezoneAsync(ulong calendarId)
        {
            var timezone = await _db.Calendars
                .Where(c => c.Id == calendarId)
                .Select(c => c.Timezone)
                .FirstOrDefaultAsync();

            return timezone;
        }

        public async Task<string> UpdateCalendarTimezoneAsync(ulong calendarId, string newTimezone)
        {
            // TODO: add checking for events rolling into the past on timezone change
            var tz = DateTimeZoneProviders.Tzdb.GetZoneOrNull(newTimezone);
            if (tz == null)
            {
                throw new InvalidTimeZoneException("Invalid TZ timezone");
            }

            var calendar = await _db.Calendars.FirstOrDefaultAsync(c => c.Id == calendarId);
            calendar.Timezone = newTimezone;

            await _db.SaveChangesAsync();
            return calendar.Timezone;
        }

        public async Task<bool?> InitialiseCalendar(ulong calendarId, string timezone, ulong defaultChannelId)
        {
            var calendar = await _db.Calendars.FirstOrDefaultAsync(c => c.Id == calendarId);
            if (!string.IsNullOrEmpty(calendar.Timezone))
            {
                return null;
            }
            var tz = DateTimeZoneProviders.Tzdb.GetZoneOrNull(timezone);
            if (tz == null)
            {
                return false;
            }
            calendar.Timezone = timezone;
            calendar.DefaultChannel = defaultChannelId;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<Calendar> TryGetCalendarAsync(ulong calendarId)
        {
            return await _db.Calendars.FirstOrDefaultAsync(c => c.Id == calendarId);
        }
    }
}