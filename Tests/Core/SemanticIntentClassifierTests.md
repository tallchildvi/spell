# SemanticIntentClassifier - Test Documentation

## Overview

Unit tests for the `SemanticIntentClassifier` class. These tests verify the correct recognition of intents 
based on semantic similarity with examples, using TF-IDF vectorization and cosine distance.

---

## Core Functionality Tests

### 1. `Classify_SimilarToReminderExample_ReturnsReminderIntent`

**Scenario:**  
User inputs a phrase similar to a reminder example but with different details.

**Importance:**  
Tests the classifier's flexibility when replacing secondary words or objects in the query.

**Expected Results:**  
- `Intent = "reminder"`  
- `Confidence > 0.55`

---

### 2. `Classify_SimilarToNoteExample_ReturnsNoteIntent`

**Scenario:**  
User formulates a request in a polite form, close to a note example.

**Importance:**  
Verifies that the system correctly ignores polite or secondary words, focusing on the key action.

**Expected Results:**  
- `Intent = "note"`  
- `Confidence > 0.55`

---

### 3. `Classify_SimilarToTimerExample_ReturnsTimerIntent`

**Scenario:**  
Timer command phrase with a different numeric value.

**Importance:**  
Verifies that the classifier doesn't bind to specific numbers and recognizes the general command structure.

**Expected Results:**  
- `Intent = "timer"`  
- `Confidence > 0.55`

---

### 4. `Classify_ExactMatch_ReturnsHighConfidence`

**Scenario:**  
Input is an exact copy of an example.

**Importance:**  
Control test to verify correctness of TF-IDF calculations and cosine similarity.

**Expected Results:**  
- `Intent = "reminder"`  
- `Confidence > 0.9`

---

### 5. `Classify_CompletelyDifferentText_ReturnsUnknown`

**Scenario:**  
Dissimilar text without common words.

**Importance:**  
Verifies that the system doesn't try to find a match where none exists and correctly returns "unknown".

**Expected Results:**  
- `Intent = "unknown"`  
- `Confidence < 0.55`

---

### 6. `Classify_DifferentWordOrder_RecognizesIntent`

**Scenario:**  
Same words but in different order.

**Importance:**  
Verifies that the model works correctly with word permutation, considering bag-of-words properties.

**Expected Results:**  
- `Intent = "reminder"`

---

### 7. `Classify_WithPunctuation_RecognizesIntent`

**Scenario:**  
Input contains punctuation and capital letters.

**Importance:**  
Verifies that punctuation and case don't affect classification results.

**Expected Results:**  
- `Intent = "reminder"`  
- `Confidence > 0.55`

---

### 8. `Classify_UpperCase_RecognizesIntent`

**Scenario:**  
Text entirely in uppercase.

**Importance:**  
Ensures classification is case-insensitive.

**Expected Results:**  
- `Intent = "reminder"`  
- `Confidence > 0.55`

---

### 9. `Classify_WithStopWords_FiltersCorrectly`

**Scenario:**  
Phrase contains many stop words.

**Importance:**  
Verifies correct stop-words filtering to avoid impact of non-essential words.

**Expected Results:**  
- `Intent = "reminder"`

---

### 10. `Classify_LongInput_HandlesGracefully`

**Scenario:**  
Very long text with many words.

**Importance:**  
Verifies that long phrases don't affect performance and stability.

**Expected Results:**  
- `Intent = "reminder"`  

---

### 11. `Classify_NumbersInText_HandlesCorrectly`

**Scenario:**  
Text contains numbers.

**Importance:**  
Verifies that numeric values are considered as meaningful tokens during classification.

**Expected Results:**  
- `Intent = "timer"`  
- `Confidence > 0.55`

---

### 12. `Classify_SpecialCharacters_FiltersAndRecognizes`

**Scenario:**  
Input contains special characters.

**Importance:**  
Verifies correct text cleaning from special characters without losing meaning.

**Expected Results:**  
- `Intent = "reminder"`

---

### 13. `Classify_WithPartialOverlap_ComputesSimilarity`

**Scenario:**  
Phrase has only partial match with examples.

**Importance:**  
Verifies that partial similarity is correctly accounted for in results.

**Expected Results:**  
- `Confidence > 0.0`

---

### 14. `Classify_MultipleIntentsWithSimilarWords_ChoosesBestMatch`

**Scenario:**  
Input contains words common to multiple intents.

**Importance:**  
Verifies that the most similar intent is chosen among possibilities.

**Expected Results:**  
- `Intent = "timer"`  
- `Confidence > 0.55`

---

### 15. `Classify_WithTypos_StillRecognizesIntent`

**Scenario:**  
Text with minor typos.

**Importance:**  
Tests classification robustness to minor spelling errors.

**Expected Results:**  
- `Intent` is not null  
- May be "reminder" or "unknown"

---

## Corner Cases

### 16. `Classify_EmptyString_ReturnsUnknown`

**Scenario:**  
Empty string.

**Importance:**  
Tests system stability to absence of input without generating errors.

**Expected Results:**  
- `Intent = "unknown"`  
- `Confidence = 0.0`

---

### 17. `Classify_WhitespaceOnly_ReturnsUnknown`

**Scenario:**  
Input consists only of whitespace.

**Importance:**  
Ensures whitespace characters are correctly processed as absence of content.

**Expected Results:**  
- `Intent = "unknown"`  
- `Confidence = 0.0`

---

### 18. `Classify_WithCustomThreshold_RespectsThreshold`

**Scenario:**  
Increased accuracy threshold set (0.9).

**Importance:**  
Verifies that threshold changes affect the decision to accept or reject results.

**Expected Results:**  
- If `Confidence < 0.9`: `Intent = "unknown"`

---

### 19. `Classify_LowSimilarity_ReturnsUnknownWithConfidence`

**Scenario:**  
Random words unrelated to examples.

**Importance:**  
Tests system behavior at low similarity to ensure adequate metrics for analytics.

**Expected Results:**  
- `Intent = "unknown"`  
- `0.0 <= Confidence < 0.55`

---

### 20. `Classify_ShortInput_HandlesGracefully`

**Scenario:**  
Single short word.

**Importance:**  
Tests system stability with minimal input.

**Expected Results:**  
- `Intent` is not null  
- `Confidence >= 0.0`

---

### 21. `Classify_OnlyStopWords_ReturnsUnknown`

**Scenario:**  
Only stop words.

**Importance:**  
Tests behavior when cleaning text that contains no meaningful tokens.

**Expected Results:**  
- `Intent = "unknown"`  
- `Confidence = 0.0`

---

### 22. `Constructor_EmptyExamples_CreatesClassifier`

**Scenario:**  
Creating classifier without examples.

**Importance:**  
Verifies stability with empty initialization (cold start).

**Expected Results:**  
- Classifier is created without exceptions

---

### 23. `AcceptanceThreshold_CanBeModified_AffectsClassification`

**Scenario:**  
Classification with different acceptance thresholds.

**Importance:**  
Verifies ability to dynamically adjust classification accuracy.

**Expected Results:**  
- With low threshold — more matches  
- With high threshold — more unknown
