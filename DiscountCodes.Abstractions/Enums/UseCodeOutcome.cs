namespace DiscountCodes.Abstractions.Enums;

public enum UseCodeOutcome : byte
{
    Success = 0, // consumed now
    Invalid = 1, // not found / bad format
    AlreadyUsed = 2  // found but previously consumed
}