using DiscountCodes.Core.Interfaces;
using DiscountCodes.Grpc;
using Google.Protobuf;
using Grpc.Core;

namespace DiscountCodes.GrpcService.Services;

public class DiscountService(IDiscountCodesService discountCodesService) : Discount.DiscountBase
{
    public override async Task<GenerateResponse> Generate(GenerateRequest request, ServerCallContext context)
    {
        if (request == null)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "request cannot be null."));

        byte length = !request.Length.IsEmpty ? request.Length[0] : (byte)0;

        var res = await discountCodesService.GenerateAsync(
            (ushort)request.Count,
            length,
            context.CancellationToken);

        var reply = new GenerateResponse { Result = res.Success };
        reply.Codes.AddRange(res.Codes); // return all generated codes
        return reply;
    }

    public override async Task<UseCodeResponse> UseCode(UseCodeRequest request, ServerCallContext context)
    {
        var outcome = await discountCodesService.UseAsync(request.Code, context.CancellationToken);
        // Convert the byte value to ByteString as required by the property type
        return new UseCodeResponse { Result = ByteString.CopyFrom([(byte)outcome]) }; // 0=success,1=invalid,2=already used
    }
}
