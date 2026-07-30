using Application.Common;
using Entities;

namespace Application.Commands.StaffAssignments;

/// <summary>
/// Command to record the completion details of a staff assignment, including
/// kilometres driven and an optional customer signature.
/// </summary>
/// <param name="AssignmentId">The identifier of the staff assignment.</param>
/// <param name="KmDriven">The number of kilometres driven by the staff member from home.</param>
/// <param name="CustomerSignature">Base64-encoded PNG of the customer's signature, or null if not signed.</param>
/// <param name="CustomerSignedName">The name of the customer representative who signed.</param>
public record RecordAssignmentCompletionCommand(
    int AssignmentId,
    int? KmDriven,
    string? CustomerSignature,
    string? CustomerSignedName) : ICommand<StaffAssignment?>;
