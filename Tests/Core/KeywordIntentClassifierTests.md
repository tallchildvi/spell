# KeywordIntentClassifier - Test Documentation

## General Overview

Unit tests for the `KeywordIntentClassifier` class. They verify the correct detection of user intents based on keywords in the command text.

---

## Core Functionality Tests

### 1. `Classify_TextWithRemindKeyword_ReturnsReminderIntent`

**Scenario:**
- User enters the command "remind me to call mom".
- The classifier should recognize the intent as "reminder".
- Confidence should be 0.7 since the keyword appears clearly.

**Importance:**
Basic functionality of reminder recognition.

**Expected Results:**
- `Intent = "reminder"`
- `Confidence = 0.7`

---

### 2. `Classify_TextWithNoteKeyword_ReturnsNoteIntent`

**Scenario:**
- User wants to save a note: "take note of this important information".
- The system should recognize this as a note creation intent.

**Importance:**
Core functionality of note recognition.

**Expected Results:**
- `Intent = "note"`
- `Confidence = 0.7`

---

### 3. `Classify_TextWithTimerKeyword_ReturnsTimerIntent`

**Scenario:**
- User enters "set timer for 10 minutes".
- Expected recognition as a timer-setting intent.

**Importance:**
The "timer" intent can be triggered by multiple keywords (timer, for). This test verifies that the combination of such words increases confidence to 0.95.

**Expected Results:**
- `Intent = "timer"`
- `Confidence = 0.95`

---

### 4. `Classify_TextWithConvertKeyword_ReturnsConvertIntent`

**Scenario:**
- User asks "convert 5 kilometers to meters".
- The system should recognize this as a unit conversion request.

**Business Logic:**
Unit conversion is a key feature for handling various types of data (distance, weight, temperature, currency, etc.). The presence of "to" raises confidence to 0.95.

**Expected Results:**
- `Intent = "convert"`
- `Confidence = 0.95`

---

## Edge Cases

### 5. `Classify_TextWithoutKeywords_ReturnsUnknownIntent`

**Scenario:**
- User enters text without any keywords: "no key words in there".
- The classifier cannot determine an intent.

**Importance:**
Boundary case – ensures proper handling of unknown input, allowing other classifiers to process it later.

**Expected Results:**
- `Intent = "unknown"`
- `Confidence = 0.0`

---

### 6. `Classify_EmptyString_ReturnsUnknownIntent`

**Scenario:**
- User enters an empty string.
- The system should return "unknown".

**Importance:**
Boundary case – protection against empty input.

**Expected Results:**
- `Intent = "unknown"`
- `Confidence = 0.0`

---

### 7. `Classify_KeywordInUpperCase_RecognizesIntent`

**Scenario:**
- User enters "REMIND me to call mom tomorrow" (uppercase).
- The system should recognize "REMIND" just like "remind".

**Importance:**
Commands should be recognized regardless of letter case.

**Expected Results:**
- `Intent = "reminder"`
- `Confidence = 0.7`

---

### 8. `Classify_KeywordInMixedCase_RecognizesIntent`

**Scenario:**
- User enters "ReMiNd me about the meeting" with mixed case.
- Should be recognized as "reminder".

**Importance:**
Case insensitivity test.

**Expected Results:**
- `Intent = "reminder"`
- `Confidence = 0.7`

---

### 9. `Classify_KeywordWithPunctuation_RecognizesIntent`

**Scenario:**
- Text includes punctuation: "Please,remind me about this!".
- Should be recognized correctly despite punctuation.

**Importance:**
Users naturally use punctuation in text. The classifier must handle it robustly.

**Expected Results:**
- `Intent = "reminder"`
- `Confidence = 0.7`

---

### 10. `Classify_TextWithMultipleKeywords_ReturnsFirstMatchedIntent`

**Scenario:**
- Text includes both "remind" and "note": "remind me to take note of this".
- Determine which intent is prioritized.

**Importance:**
Tests behavior when multiple keywords are present. Depending on implementation:
- Could select the first found keyword.
- Could prioritize based on importance.

**Assumption:**
"remind" has higher or first-found priority.

**Expected Results:**
- `Intent = "reminder"`
- `Confidence = 0.7`

---

### 11. `Classify_PartialKeywordMatch_DoesNotTriggerIntent`

**Scenario:**
- Text includes partial word match: "this is a remainder of the calculation".
- "remainder" (math term) should not trigger "reminder".

**Importance:**
Precision test. Prevents false positives such as:
- "remainder" → "reminder"
- "conversion" → "convert"

**Expectation:**
Classifier should match whole words, not substrings.

**Expected Results:**
- `Intent = "unknown"`
- `Confidence = 0.0`

