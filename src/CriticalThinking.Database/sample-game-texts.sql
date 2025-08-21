-- Sample Game Texts with Embedded Fallacies
-- These texts are designed to be coherent while containing specific logical fallacies

-- Easy Level Game Texts (3 fallacies each)

-- Easy Game Text 1: School Debate
INSERT INTO game_texts (title, full_text, difficulty, target_fallacy_count) VALUES
('The School Funding Debate', 
'During yesterday''s town hall meeting about school funding, local businessman Tom Richardson argued against the proposed education budget increase. "We can''t trust anything Sarah Martinez says about education," Richardson declared, "she doesn''t even have children of her own, so what does she know about schools?" He continued, "Martinez claims we need more funding, but what she really wants is to waste all our taxpayer money on unnecessary luxuries." The crowd murmured approval when Richardson added, "Everyone in this room who cares about fiscal responsibility opposes this budget increase. You''re either with responsible spending or you''re against it - there''s no middle ground here."', 
'Easy', 3);

-- Easy Game Text 2: Health Discussion  
INSERT INTO game_texts (title, full_text, difficulty, target_fallacy_count) VALUES
('The Organic Food Controversy',
'At the community health fair, wellness blogger Jenny Chen made passionate arguments for organic food. "Think of your children''s future!" she exclaimed, "How can you feed them food filled with dangerous chemicals?" When questioned about the higher costs, Chen responded, "Dr. Williams, the famous TV personality, swears by organic foods, so they must be healthier." She then addressed skeptics in the audience: "Everyone who truly cares about their family''s health buys organic. Either you prioritize your family''s wellbeing or you don''t - it''s that simple." The crowd was moved by her emotional appeal about protecting innocent children from harm.',
'Easy', 3);

-- Easy Game Text 3: Technology Debate
INSERT INTO game_texts (title, full_text, difficulty, target_fallacy_count) VALUES
('The Social Media Debate',
'At the parent-teacher conference, concerned mother Lisa Thompson spoke against allowing social media in schools. When tech-savvy parent Mike Johnson disagreed, Thompson shot back, "How can you defend social media when your own teenager was caught cyberbullying last year?" She continued with passion, "Just imagine if our precious children become addicted to their phones and lose all ability to communicate face-to-face! We must ban social media completely." Thompson concluded her argument by stating, "All the parents at Roosevelt Elementary agree with me - everyone knows social media is destroying our youth."',
'Easy', 3);

-- Medium Level Game Texts (6 fallacies each)

-- Medium Game Text 1: Environmental Policy
INSERT INTO game_texts (title, full_text, difficulty, target_fallacy_count) VALUES
('The Climate Action Proposal',
'City Council member David Park presented his climate action plan at Tuesday''s meeting. "Every responsible city has implemented green policies," Park began, "so we must follow suit to stay relevant." When asked for evidence, Park replied, "I don''t need to prove climate change is real - those who deny it need to prove it''s not happening." He shared a personal story: "My neighbor switched to solar panels and his electricity bill disappeared completely, proving that renewable energy saves money for everyone." Park dismissed critics by saying, "No true environmentalist would oppose clean energy initiatives." He added, "Since carbon emissions increased after we built the new highway, clearly highways cause climate change." The council ultimately decided to find a reasonable middle ground between Park''s comprehensive plan and opponents'' preference for no action at all.',
'Medium', 6);

-- Medium Game Text 2: Healthcare Discussion
INSERT INTO game_texts (title, full_text, difficulty, target_fallacy_count) VALUES
('The Alternative Medicine Debate',
'Dr. Rebecca Santos defended alternative medicine at the medical conference. "Herbal remedies are completely natural," she argued, "so they''re obviously safer than synthetic drugs with artificial chemicals." When challenged about scientific evidence, Santos responded, "Critics can''t prove these treatments don''t work, which shows their effectiveness." She cited personal experience: "My grandmother used turmeric for arthritis and lived to 95, proving that natural remedies extend lifespan." Santos dismissed pharmaceutical researchers, claiming, "No real healer would prioritize profits over natural healing methods." She noted, "After our clinic started offering acupuncture, patient satisfaction increased, clearly demonstrating that acupuncture improves health outcomes." The conference concluded by seeking a compromise between traditional medicine and Santos''s holistic approach.',
'Medium', 6);

-- Hard Level Game Texts (9 fallacies each)

-- Hard Game Text 1: Complex Policy Discussion
INSERT INTO game_texts (title, full_text, difficulty, target_fallacy_count) VALUES
('The Urban Development Proposal',
'At the heated city planning meeting, developer Marcus Chen presented his controversial mixed-use project. "This development is necessary for progress," Chen began, "because progress is essential for our city''s growth." When residents expressed concerns, Chen challenged them: "Have you stopped opposing beneficial development in our neighborhood?" He dismissed environmental worries by stating, "Since I cannot understand how a small development could possibly impact the entire ecosystem, these environmental claims must be false." Chen cited statistical support: "Three successful developments in the past decade prove this project will succeed," carefully omitting five failed projects from the same period. When questioned about funding sources, Chen deflected: "Critics can''t prove my financing is problematic, therefore it must be legitimate." He attacked opposition leader Sarah Kim personally: "Kim''s ideas come from San Francisco''s planning department, and everyone knows San Francisco policies are fundamentally flawed." Chen dismissed expert concerns by labeling them: "No true urban planner would oppose smart development like this." He noted coincidental timing: "Property values increased after we announced this project, proving community support for development." Finally, he demanded compromise: "The planning commission should find middle ground between my complete vision and opponents'' total rejection."',
'Hard', 9);

-- Insert the fallacy mappings for each game text

-- Easy Game Text 1 mappings (Ad Hominem, Strawman, False Dilemma)
INSERT INTO game_text_fallacies (game_text_id, fallacy_id) VALUES
(1, (SELECT id FROM logical_fallacies WHERE key = 'ad-hominem')),
(1, (SELECT id FROM logical_fallacies WHERE key = 'strawman')),
(1, (SELECT id FROM logical_fallacies WHERE key = 'false-dilemma'));

-- Easy Game Text 2 mappings (Appeal to Emotion, Appeal to Authority, Bandwagon)  
INSERT INTO game_text_fallacies (game_text_id, fallacy_id) VALUES
(2, (SELECT id FROM logical_fallacies WHERE key = 'appeal-to-emotion')),
(2, (SELECT id FROM logical_fallacies WHERE key = 'appeal-to-authority')),
(2, (SELECT id FROM logical_fallacies WHERE key = 'bandwagon'));

-- Easy Game Text 3 mappings (Tu Quoque, Slippery Slope, Bandwagon)
INSERT INTO game_text_fallacies (game_text_id, fallacy_id) VALUES
(3, (SELECT id FROM logical_fallacies WHERE key = 'tu-quoque')),
(3, (SELECT id FROM logical_fallacies WHERE key = 'slippery-slope')),
(3, (SELECT id FROM logical_fallacies WHERE key = 'bandwagon'));

-- Medium Game Text 1 mappings (Bandwagon, Burden of Proof, Anecdotal, No True Scotsman, Post Hoc, Middle Ground)
INSERT INTO game_text_fallacies (game_text_id, fallacy_id) VALUES
(4, (SELECT id FROM logical_fallacies WHERE key = 'bandwagon')),
(4, (SELECT id FROM logical_fallacies WHERE key = 'burden-of-proof')),
(4, (SELECT id FROM logical_fallacies WHERE key = 'anecdotal')),
(4, (SELECT id FROM logical_fallacies WHERE key = 'no-true-scotsman')),
(4, (SELECT id FROM logical_fallacies WHERE key = 'post-hoc')),
(4, (SELECT id FROM logical_fallacies WHERE key = 'middle-ground'));

-- Medium Game Text 2 mappings (Appeal to Nature, Appeal to Ignorance, Anecdotal, No True Scotsman, Post Hoc, Middle Ground)
INSERT INTO game_text_fallacies (game_text_id, fallacy_id) VALUES
(5, (SELECT id FROM logical_fallacies WHERE key = 'appeal-to-nature')),
(5, (SELECT id FROM logical_fallacies WHERE key = 'appeal-to-ignorance')),
(5, (SELECT id FROM logical_fallacies WHERE key = 'anecdotal')),
(5, (SELECT id FROM logical_fallacies WHERE key = 'no-true-scotsman')),
(5, (SELECT id FROM logical_fallacies WHERE key = 'post-hoc')),
(5, (SELECT id FROM logical_fallacies WHERE key = 'middle-ground'));

-- Hard Game Text 1 mappings (All 9 hard-level fallacies)
INSERT INTO game_text_fallacies (game_text_id, fallacy_id) VALUES
(6, (SELECT id FROM logical_fallacies WHERE key = 'begging-the-question')),
(6, (SELECT id FROM logical_fallacies WHERE key = 'loaded-question')),
(6, (SELECT id FROM logical_fallacies WHERE key = 'personal-incredulity')),
(6, (SELECT id FROM logical_fallacies WHERE key = 'texas-sharpshooter')),
(6, (SELECT id FROM logical_fallacies WHERE key = 'appeal-to-ignorance')),
(6, (SELECT id FROM logical_fallacies WHERE key = 'genetic-fallacy')),
(6, (SELECT id FROM logical_fallacies WHERE key = 'no-true-scotsman')),
(6, (SELECT id FROM logical_fallacies WHERE key = 'post-hoc')),
(6, (SELECT id FROM logical_fallacies WHERE key = 'middle-ground'));