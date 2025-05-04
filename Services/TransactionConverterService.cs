using Application.Interfaces.Services;
using Domain.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    public class TransactionConverterService : ITransactionConverterService
    {
        public BlockTransaction ConvertToBlockTransaction(PendingVote pendingVote)
        {
            var transaction = new BlockTransaction
            {
                PollId = pendingVote.PollId,
                VoteId = pendingVote.VoteId,
                UserIdHash = ComputeSha256Hash($"{pendingVote.PollId}:{pendingVote.UserId}"),
                CandidateIds = pendingVote.CandidateIds,
                Timestamp = pendingVote.Timestamp
            };
            transaction.Hash = ComputeTransactionHash(transaction);
            return transaction;
        }

        private string ComputeSha256Hash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes).ToLower();
        }

        private string ComputeTransactionHash(BlockTransaction transaction)
        {
            var data = new
            {
                transaction.PollId,
                transaction.VoteId,
                transaction.UserIdHash,
                transaction.CandidateIds,
                transaction.Timestamp
            };
            return ComputeSha256Hash(JsonSerializer.Serialize(data));
        }
    }
}
