# Hermes 3.4.3 Fix Roadmap

This roadmap is the handoff document for fixing the remaining HermesProxy WotLK-to-Classic compatibility issues. It is intentionally repo-local and portable: do not add machine-specific paths, launch commands, or private checkout locations here.

## Workflow

- Keep `master` stable.
- Merge verified fixes into `develop`.
- Use one branch per fix or diagnostic experiment.
- Treat broad gates and packet suppressions as diagnostic until narrowed to a field-level or packet-level fix.
- Build from the repo root with a repo-relative command such as `dotnet publish HermesProxy/HermesProxy.csproj -c Release -r win-x64 --self-contained true -o build`.
- When a crash, disconnect, or UI defect appears, preserve the latest Hermes log, packet log, and client dump/sniff before changing the code again.

## Current Test Branches

| Branch | Status | Issue | Verification target |
| --- | --- | --- | --- |
| `fix/pet-power-regen-slots` | Awaiting game test | Playerbot/pet party disconnects around pet Values updates. Maps legacy pet/class power regen fields into modern power slots. | Party with bots that have pets; fight and move together without reason-7 disconnects or crashes. |
| `fix/container-values-buyback` | Awaiting game test | Sold items stay grey in bags and buyback does not populate immediately. Adds modern `ContainerData` updates and a direct self-slot clear. | Sell items from different bags; bag slot clears, buyback row appears, relog is not required. |
| `fix/npc-interaction-stop` | Awaiting game test | Moving/traveling vendors continue walking while gossip/vendor window is open. Sends a local stop movement update on interaction. | Open gossip/vendor/trainer on a moving NPC; NPC stops locally and interaction remains usable. |
| `diagnostic/non-self-player-values-gate` | Diagnostic only | Broadly skips non-self player Values updates to avoid crashes/disconnects. | Use only to prove the unsafe packet class; replace with a narrow writer fix before `develop`. |
| `test/current-hermes-runtime-fixes` | Backup/reference only | Mixed snapshot of current experiments. | Do not merge directly; split into verified branches first. |

## High Priority Tickets

### HERMES-001: Replace Non-Self Player Values Gate

The current broad non-self player Values skip is useful for proving that playerbot/player Values updates are unsafe, but it is not a real fix. The next proper move is to decode a failing non-self player Values update field-by-field against a known-good 3.4.3 reference writer and identify the exact width, block, or dynamic-array divergence.

Evidence:
- Latest logs repeatedly show `[NonSelfPlayerValues] Skipped Values update`.
- Earlier testing showed the paladin became stable when the broad post-login gate was disabled only after the guild/packet fixes landed.
- Bot party and pet tests still touch this area.

Done means:
- The broad non-self player gate is removed.
- The exact bad field or packet block is fixed.
- A playerbot party can move, fight, update auras, update health/power, and disconnect/logout normally.

### HERMES-002: Finish Bot Pet Values Support

Pet Values are now isolated enough to have their own branch. The known suspicious area is modern `UnitData` power slot alignment: legacy pets expose class-specific power values, while modern clients expect compact power arrays by slot.

Evidence:
- Latest logs show `[NonSelfPetValues] Skipped Values update`.
- Latest logs also show `Pet name query response for unknown pet 25751!`, which may be harmless, but should be tracked if pet UI/name behavior is wrong.

Done means:
- Bot pet Values are not skipped broadly.
- Hunter/warlock/bot pet parties remain stable in combat.
- Pet names, health, power, and aura updates look sane.

### HERMES-003: Vendor Sell and Buyback State

Selling currently needs modern inventory/container state to update immediately. The branch adds direct slot clearing and `ContainerData` updates, but needs in-game confirmation.

Evidence:
- User saw sold items remain grey in bags until relog.
- User saw vendor recent sold/buyback not update.

Done means:
- Sold item disappears from the bag immediately.
- Buyback/recent sold slot updates without relog.
- The fix works for backpack and extra bags.

### HERMES-004: Moving NPC Interaction Stop

Moving vendors can continue along their path while the interaction window is open. A local stop update may be enough, but this should be verified against gossip, vendor, quest, and trainer windows.

Evidence:
- User reported a traveling salesman did not stop when opening the shop menu.

Done means:
- Moving NPCs stop locally while interacted with.
- Vendor/gossip/trainer/quest windows stay usable.
- No position snap or movement corruption after closing the window.

### HERMES-005: Transport Creates and Old-Style Transport Filter

Transports are still deliberately filtered because old-style transport creates previously crashed the modern client. This is a known feature gap, not a final behavior.

Evidence:
- `research/transport_crash_investigation.md`
- Latest logs show many `[Transport] FILTERED old-style transport entry ...` lines.
- Code still contains transport debug/filter behavior.

Likely fix areas:
- Populate legacy `GAMEOBJECT_BYTES_1` TypeID/State correctly.
- Verify movement rotation/quaternion order for modern create packets.
- Remove the filter only after a field-level comparison passes.

Done means:
- Boats, zeppelins, elevators, and similar transports appear and move without crashes.
- Transport filtering/debug spam can be removed or downgraded.

### HERMES-006: Death and Revive Visual State

Death/revive visual state still has a research trail suggesting incremental Values updates are not enough for the modern client. A self-object recreate path on revive may be needed.

Evidence:
- `research/death_revive_visual.md`
- Existing notes indicate grey ghost/death overlay can persist after revive.

Done means:
- Die, release, revive, and resurrect flows clear ghost/death visuals without relog.
- Health, movement, corpse, and aura state remain correct after revive.

## Medium Priority Tickets

### HERMES-007: Quest POI and Map Accuracy

The conservative quest POI/map fixes got the UI working, but the long-term mapping should be more accurate.

Evidence:
- Verified fixes include explored zones and conservative quest POI changes.
- User saw map and quest markers improve over time.

Done means:
- Explored map areas load correctly.
- Quest markers/POIs appear in the expected zones.
- If needed, add a proper legacy area-to-modern UiMap mapping rather than relying on conservative zero/default fields.

### HERMES-008: Loot Rolls and Group Loot

The opcode tracker still records dropped loot roll packets.

Evidence:
- `missing_opcodes.log` has `SMSG_LOOT_START_ROLL` mapped to opcode 0.

Done means:
- Group loot rolls appear in the modern client.
- Need/greed/pass choices reach the legacy server.
- No opcode-0 drops for loot roll packets.

### HERMES-009: Spell Execute Log and Combat Detail Packets

The opcode tracker repeatedly records unhandled `SMSG_SPELL_EXECUTE_LOG`.

Evidence:
- `missing_opcodes.log` repeatedly lists `SMSG_SPELL_EXECUTE_LOG`.

Done means:
- Spell execute details are converted or safely ignored only when proven non-user-visible.
- Combat text, proc feedback, and cast side effects remain correct.

### HERMES-010: LFG Search Updates

LFG search updates are still unhandled.

Evidence:
- `missing_opcodes.log` repeatedly lists `SMSG_LFG_UPDATE_SEARCH`.

Done means:
- LFG search/update UI either works or is intentionally hidden/disabled for unsupported legacy behavior.
- No repeated missing opcode entries for normal play.

### HERMES-011: Dismount Conversion

The opcode tracker records unhandled legacy dismount packets.

Evidence:
- `missing_opcodes.log` lists `SMSG_DISMOUNT`.

Done means:
- Mount/dismount state updates correctly from server-driven dismounts.
- The modern client does not rely only on aura or movement side effects.

### HERMES-012: GameObject Interaction Regression Suite

Opening, gathering, mining, herbs, chests, and quest objects all touch the same target-report/cast timing path. Several fixes have landed, but this needs a compact regression checklist.

Evidence:
- User previously saw first gather attempt return "not in range".
- User saw chest open fail with "invalid target".
- Verified fixes include deferred game object casts and opening spell target report handling.

Done means:
- First attempt works for herbs, mining nodes, chests, and quest game objects.
- No false "not in range" or "invalid target" on valid targets.

## Lower Priority / Reopen Only If Symptoms Return

### HERMES-013: Active Player Create Dynamic Tail

Older crash analysis suspected self ActivePlayer dynamic fields. Later tests made the paladin stable after other fixes, so this is historical unless the same client crash signature returns.

Reopen if:
- The client crashes on login with the same dynamic-array or large loop signature.
- A fresh sniff/dump points back to ActivePlayer CreateObject alignment.

### HERMES-014: Generated Item Hotfix / Item Render Path

Old investigations saw item data and render/OOM symptoms. The item create tail and related packet issues were later fixed, so this should stay dormant unless new item-render crashes appear.

Reopen if:
- Client log shows item hotfix or jam mirror failures.
- Equipping/seeing a specific item causes render corruption, unknown textures, or OOM.

### HERMES-015: Creature Health Visual Edge Cases

Creature health visuals had historical notes around empty Unit update blocks and `hasAny` handling. Many update-field fixes already landed, but creature/bot combat should keep this on the watch list.

Reopen if:
- Creature health bars stop updating.
- Combat state desyncs while logs show suspicious UnitData block masks.

## Tooling Tickets

### HERMES-016: Portable Crash/Sniff Decode Notes

The experiment process worked best when using Hermes logs, packet sniffs, minidumps, and a Wrathion reference checkout together. That method should become a small portable guide or script set inside the repo.

Done means:
- A new contributor can find latest logs/sniffs/dumps without local hardcoded paths.
- The guide explains how to compare Hermes writers against Wrathion or another known-good reference.
- Any helper scripts accept paths as arguments and do not assume a specific machine layout.

### HERMES-017: Debug Log Cleanup

Many debug gates and log lines were added while diagnosing crashes. Keep useful breadcrumbs, but remove or downgrade noisy temporary logs after each fix is verified.

Done means:
- Normal play logs are readable.
- Diagnostic branches still add explicit targeted logs when needed.
- `develop` does not carry broad noisy gates unless intentionally documented.

## Evidence Map

- `research/update_fields_audit.md`: broad update-field audit; some entries are stale and must be checked against current `develop`.
- `research/transport_crash_investigation.md`: transport crash/filter history.
- `research/player_values_update_crash.md`: player Values crash/disconnect investigation.
- `research/death_revive_visual.md`: death/revive visual-state investigation.
- `research/creature_health_visual_updates.md`: creature health visual investigation.
- `build/logs/missing_opcodes.log`: current repeated opcode gaps from local testing.
- `build/logs/*.txt`: Hermes runtime logs from local test sessions.
- `build/PacketsLog/*.pkt`: packet captures from local test sessions.

## Standard Investigation Loop

1. Start from `develop` and create a focused branch.
2. Reproduce the smallest visible symptom.
3. Capture Hermes log, packet log, and client crash dump if present.
4. Compare the relevant Hermes writer/reader against a known-good reference implementation.
5. Fix the narrowest packet or field mismatch.
6. Build and have the user verify in game.
7. Remove temporary gates/log spam.
8. Merge to `develop` only after the user reports the fix is verified.
