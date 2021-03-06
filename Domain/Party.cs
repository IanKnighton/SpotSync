﻿using SpotSync.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Domain
{
    public class Party
    {
        public Playlist Playlist;
        public Guid Id { get; }
        public PartyGoer Host { get; }
        public List<PartyGoer> Listeners { get; }
        public string PartyCode { get; }
        private const int LENGTH_OF_PARTY_CODE = 6;

        public Party(PartyGoer host)
        {
            Id = Guid.NewGuid();
            Host = host;
            Listeners = new List<PartyGoer>() { host };
            PartyCode = GeneratePartyCode();
        }

        public async Task ModifyPlaylistAsync(RearrangeQueueRequest request)
        {
            await Playlist.ModifyQueueAsync(request);
        }

        public async Task ModifyPlaylistAsync(AddSongToQueueRequest request)
        {
            await Playlist.AddSongToQueueAsync(request);
        }

        public async Task DeletePlaylistAsync()
        {
            await Playlist.DeleteAsync();
            Playlist = null;
        }

        public bool IsPartyPlayingMusic() => Playlist?.CurrentSong != null;

        public void JoinParty(PartyGoer partyGoer)
        {
            Listeners.Add(partyGoer);
        }

        private static string GeneratePartyCode()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            // generate 6 random characters
            return new string(Enumerable.Repeat(chars, LENGTH_OF_PARTY_CODE).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
