-- Logical Fallacies Data
-- Categorized by difficulty: Easy (8), Medium (8), Hard (8)

-- Easy Fallacies (8) - Clear, obvious, commonly understood
INSERT INTO logical_fallacies (name, key, description, difficulty, example) VALUES
('Ad Hominem', 'ad-hominem', 'Attacking the person making the argument rather than the argument itself.', 'Easy', 'You can''t trust John''s opinion on climate change because he''s not a scientist.'),
('Strawman', 'strawman', 'Misrepresenting someone''s argument to make it easier to attack.', 'Easy', 'Person A: We should have better funding for schools. Person B: Why do you want to waste money?'),
('Appeal to Authority', 'appeal-to-authority', 'Using the opinion of an authority figure, or institution of authority, in place of an actual argument.', 'Easy', 'Dr. Smith says this medicine works, so it must be true (when Dr. Smith is not a medical doctor).'),
('False Dilemma', 'false-dilemma', 'Presenting two alternative states as the only possibilities when more possibilities exist.', 'Easy', 'You''re either with us or against us.'),
('Slippery Slope', 'slippery-slope', 'Asserting that if we allow A to happen, then Z will consequently happen too.', 'Easy', 'If we allow students to redo this test, soon they''ll want to redo every assignment.'),
('Appeal to Emotion', 'appeal-to-emotion', 'Manipulating an emotional response in place of a valid or compelling argument.', 'Easy', 'Think of the children! We must ban this immediately.'),
('Bandwagon', 'bandwagon', 'Appealing to popularity or the fact that many people do something as an attempted form of validation.', 'Easy', 'Everyone is buying this product, so it must be good.'),
('Tu Quoque', 'tu-quoque', 'Avoiding having to engage with criticism by turning it back on the accuser.', 'Easy', 'How can you criticize my driving when you got a speeding ticket last year?');

-- Medium Fallacies (8) - Require more understanding of logic
INSERT INTO logical_fallacies (name, key, description, difficulty, example) VALUES
('Burden of Proof', 'burden-of-proof', 'Claiming that the burden of proof lies not with the person making the claim, but with someone else to disprove.', 'Medium', 'I don''t have to prove God exists. You have to prove he doesn''t.'),
('No True Scotsman', 'no-true-scotsman', 'Making what could be called an appeal to purity as a way to dismiss relevant criticisms.', 'Medium', 'No true Christian would support that policy.'),
('The Texas Sharpshooter', 'texas-sharpshooter', 'Cherry-picking data clusters to suit an argument, or finding a pattern to fit a presumption.', 'Medium', 'Looking at a wall full of bullet holes and drawing a target around the tightest cluster.'),
('Appeal to Nature', 'appeal-to-nature', 'Making the argument that because something is ''natural'' it is therefore valid, justified, inevitable, or good.', 'Medium', 'This herbal remedy is natural, so it must be safe and effective.'),
('Composition/Division', 'composition-division', 'Assuming that what''s true about one part of something has to be applied to all parts of it.', 'Medium', 'Each player on this team is excellent, so the team must be excellent.'),
('Anecdotal', 'anecdotal', 'Using personal experience or an isolated example instead of compelling evidence.', 'Medium', 'My grandfather smoked and lived to 90, so smoking isn''t harmful.'),
('Post Hoc', 'post-hoc', 'Assuming that because B comes after A, A caused B.', 'Medium', 'I wore my lucky socks and we won the game, so the socks caused our victory.'),
('Middle Ground', 'middle-ground', 'Claiming that a compromise between two positions is always correct.', 'Medium', 'Some say the earth is round, others say it''s flat, so it must be cylindrical.');

-- Hard Fallacies (8) - Complex, subtle, require deep understanding
INSERT INTO logical_fallacies (name, key, description, difficulty, example) VALUES
('Begging the Question', 'begging-the-question', 'A circular argument in which the conclusion is included in the premise.', 'Hard', 'The Bible is true because it says so in the Bible.'),
('Special Pleading', 'special-pleading', 'Moving the goalposts to create exceptions when a claim is shown to be false.', 'Hard', 'Psychic predictions work, except when tested under controlled conditions because the skeptical energy interferes.'),
('Appeal to Ignorance', 'appeal-to-ignorance', 'Claiming that because something can''t be proven false, it must be true.', 'Hard', 'No one has proven that aliens don''t exist, so they must exist.'),
('Loaded Question', 'loaded-question', 'Asking a question that has a presumption built into it so that it can''t be answered without appearing guilty.', 'Hard', 'Have you stopped beating your wife?'),
('Ambiguity', 'ambiguity', 'Using double meanings or ambiguities of language to mislead or misrepresent the truth.', 'Hard', 'The sign said ''fine for parking here'' so I thought it was okay to park.'),
('The Gambler''s Fallacy', 'gamblers-fallacy', 'Believing that ''runs'' occur to statistically independent phenomena such as roulette wheel spins.', 'Hard', 'Red has come up 5 times in a row, so black is due to come up next.'),
('Personal Incredulity', 'personal-incredulity', 'Claiming that because one finds something difficult to understand, it''s therefore not true.', 'Hard', 'I can''t understand how evolution works, so it must be false.'),
('Genetic Fallacy', 'genetic-fallacy', 'Judging something as either good or bad on the basis of where it comes from, or from whom it came.', 'Hard', 'This idea came from a communist country, so it must be bad.');