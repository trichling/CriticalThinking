-- Critical Thinking Game Database Schema

-- Logical Fallacies table
CREATE TABLE logical_fallacies (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    key VARCHAR(50) NOT NULL UNIQUE,
    description TEXT NOT NULL,
    difficulty VARCHAR(10) NOT NULL CHECK (difficulty IN ('Easy', 'Medium', 'Hard')),
    example TEXT NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Text building blocks - reusable text fragments that contain specific fallacies
CREATE TABLE text_blocks (
    id SERIAL PRIMARY KEY,
    fallacy_id INTEGER NOT NULL REFERENCES logical_fallacies(id),
    content TEXT NOT NULL,
    context VARCHAR(100), -- e.g., 'politics', 'science', 'everyday'
    position_hint VARCHAR(20) DEFAULT 'any', -- 'beginning', 'middle', 'end', 'any'
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Pre-composed complete texts for games
CREATE TABLE game_texts (
    id SERIAL PRIMARY KEY,
    title VARCHAR(200),
    full_text TEXT NOT NULL,
    difficulty VARCHAR(10) NOT NULL CHECK (difficulty IN ('Easy', 'Medium', 'Hard')),
    target_fallacy_count INTEGER NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Junction table linking game texts to their fallacies
CREATE TABLE game_text_fallacies (
    id SERIAL PRIMARY KEY,
    game_text_id INTEGER NOT NULL REFERENCES game_texts(id),
    fallacy_id INTEGER NOT NULL REFERENCES logical_fallacies(id),
    text_position_start INTEGER, -- character position where fallacy starts
    text_position_end INTEGER, -- character position where fallacy ends
    UNIQUE(game_text_id, fallacy_id)
);

-- Game sessions
CREATE TABLE game_sessions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    player_name VARCHAR(100) NOT NULL,
    difficulty VARCHAR(10) NOT NULL CHECK (difficulty IN ('Easy', 'Medium', 'Hard')),
    game_text_id INTEGER NOT NULL REFERENCES game_texts(id),
    started_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    completed_at TIMESTAMP WITH TIME ZONE,
    time_taken_seconds INTEGER,
    score INTEGER
);

-- Player's answers for a game session
CREATE TABLE game_answers (
    id SERIAL PRIMARY KEY,
    session_id UUID NOT NULL REFERENCES game_sessions(id),
    fallacy_id INTEGER NOT NULL REFERENCES logical_fallacies(id),
    is_correct BOOLEAN NOT NULL,
    answer_type VARCHAR(20) NOT NULL CHECK (answer_type IN ('correct', 'wrong', 'missed')),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Indexes for better performance
CREATE INDEX idx_logical_fallacies_difficulty ON logical_fallacies(difficulty);
CREATE INDEX idx_text_blocks_fallacy_id ON text_blocks(fallacy_id);
CREATE INDEX idx_text_blocks_difficulty ON text_blocks((SELECT difficulty FROM logical_fallacies WHERE id = fallacy_id));
CREATE INDEX idx_game_texts_difficulty ON game_texts(difficulty);
CREATE INDEX idx_game_sessions_player_name ON game_sessions(player_name);
CREATE INDEX idx_game_sessions_started_at ON game_sessions(started_at);
CREATE INDEX idx_game_answers_session_id ON game_answers(session_id);