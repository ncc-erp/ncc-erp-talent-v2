# English interview results on Talent

Talent stores AI interview results on the candidate profile. HR opens the
**EnglishInterview** tab for the latest interview's scores and detailed feedback.
This flow sends short Komu DMs to the candidate and HR. It does not send email,
create email history or seed email templates.

## Bot contract

- `POST {host}/api/services/app/Public/ReceiveAIInterviewResult`
- Headers: `X-Secret-Key: <App.AIInterviewWebhookSecret>` and `Content-Type: application/json`.
- Optional `Abp-TenantName` selects a tenant.
- Required fields: `candidate_email`, `session_id`.
- `user_id` is the candidate's Mezon ID for the candidate DM. Send it as a
  **string** to preserve the full ID. If missing, Talent still saves results
  and attempts the HR DM, but records the candidate notification failure.
- Other fields: `candidate_name`, `timestamp`, `coach_name`, `scenario_id`,
  `scenario_title`, `stats.duration_seconds`, `report`.

The existing payload is unchanged. Talent stores `report` except for
`words_breakdown`; individual scores also have separate columns.

| Case | HTTP | Behavior |
|---|---|---|
| Missing/wrong secret, or server secret not configured | 401 | Reject |
| Missing email or session ID | 400 | Reject |
| Valid result | 200 | ABP `result: { success, message, resultId }` |
| Repeated session ID | 200 | Return existing result; no repeated DMs |
| Candidate not found | 200 | Store with `CandidateNotFound` |
| DM failed | 200 | Keep result; save `NotifyFailed` and error in `Note` |

## Processing

1. Validate and deduplicate by `SessionId`.
2. Match the latest non-clone CV by case-insensitive candidate email.
3. Resolve HR from `CV.LastModifierUser`, falling back to `CV.CreatorUser`.
4. Match the scheduled RequestCV nearest the interview timestamp, or the newest
   RequestCV if none is scheduled. Set `Interviewed = true` only; do not change
   its status or salary percentage.
5. Save `AIInterviewResult` linked to the CV and optional RequestCV.
6. Send both DMs independently and await Komu's HTTP responses. Failure in one
   DM does not prevent attempting the other.

## Komu DM contract

Both use `POST sendMessageToUser` with the existing Komu credentials:

```json
{ "userId": "1942867974385831936", "message": "Short candidate notification" }
```

```json
{ "username": "hr.username", "message": "Short HR notification with Talent profile link" }
```

Komu accepts either `userId` (the Mezon user ID) or `username`. The JSON field
is `userId`, not `mezonId`. Talent sends candidates by ID and HR by username.
Komu awaits `sendDM` before returning success. HTTP failures are recorded;
success does not indicate that the recipient has read the message.

The HR DM links to
`app/candidate/{staff-list|intern-list}/{cvId}?userType={type}&tab=5`.
It contains candidate identity, overall score, CEFR and the detail link.
The candidate receives a brief confirmation with overall score and CEFR.

## Profile API and UI

- `GET {host}/api/services/app/EnglishInterview/GetByCVId?cvId={id}`.
- Requires login and the existing **Personal Info / View** permission for the
  actual CV type (Intern or Staff), matching the tab content permission.
- Normal tenant and soft-delete filters apply to candidate lookup and results.
- Returns an array in ABP's `result` wrapper, newest interview first (creation
  time is the fallback if the interview date is absent).
- Items include `id`, `sessionId`, `scenarioTitle`, `coachName`, `interviewedAt`,
  `durationSeconds`, all score fields, `cefrLevel` and `report` as an object.
  Internal notification notes and candidate Mezon IDs are excluded.
- EnglishInterview is tab index 5 for Staff and Intern. It is disabled until
  the candidate is saved. Opening the tab or clicking Refresh reloads results.
- The tab displays only the newest interview, without an accordion. Previous
  sessions remain stored. Repeated delivery of the same session does not create
  another result or send notifications again.
- Overall score and CEFR are highlighted alongside IELTS/TOEFL scores, followed
  by five skill scores. Speaking rate and filler count appear as compact metrics.
- Interview feedback displays only **Key strengths** and **Areas to improve**
  in two columns. Other report sections and technical IDs are hidden from the UI.
  The stored report remains unchanged.
- Loading, empty and error states have separate messages.

## Database and configuration

- Existing migration: `20261106111106_Add_AIInterviewResult_table`.
- Table: `AIInterviewResults`, with `NccAuditEntity` and `IMayHaveTenant`.
- `App.AIInterviewWebhookSecret`: webhook authentication.
- `App:ClientRootAddress`: profile links in HR DMs.
- `KomuService`: base address and credentials for DMs.
- No SMTP configuration or email templates are required for this flow.
