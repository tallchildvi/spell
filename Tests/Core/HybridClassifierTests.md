# HybridClassifier - Test Documentation

## Overview

Tests for the 'HybridClassifier' class using 'KeywordIntentClassifier' (primary) and 'SemanticIntentClassifier' (fallback). 

---

## Core Functionality Tests

### 1. 'Classify_KeywordMatch_ReturnsPrimaryResult'

**Scenario:**  
Input contains exact keyword match

**Importance:**  
Verifies that keyword-based classification works correctly and returns results with high confidence when exact keywords are detected.

**Expected Results:**  
- 'Intent = "reminder"'
- 'Confidence > PrimaryConfidenceThreshold = 0.5'

---

### 2. 'Classify_NoKeywordButSemanticMatch_ReturnsFallbackResult'

**Scenario:**  
Input has no exact keywords but is semantically similar to training examples.

**Importance:**  
Tests the main hybrid logic - automatic fallback to semantic classifier when keyword-based approach fails or returns low confidence.

**Expected Results:**  
- 'Intent = "note"'
- 'Confidence > 0.0'
- Result comes from semantic classifier

---

### 3. 'Classify_CompletelyUnknown_ReturnsUnknown'

**Scenario:**  
Input doesn't match any keywords or semantic patterns.

**Importance:**  
Verifies correct behavior when both classifiers fail to identify a known intent. System should gracefully return "unknown" without errors.

**Expected Results:**  
- 'Intent = "unknown"'

---

## Corner Cases

### 4. 'Classify_EmptyInput_HandlesGracefully'

**Scenario:**  
Empty string input.

**Importance:**  
Tests system stability with edge case input. Verifies that hybrid classifier delegates empty input handling to underlying classifiers without crashing.

**Expected Results:**  
- 'Intent = "unknown"'