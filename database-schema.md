# Tashrif Database Schema

Complete schema covering all entities used in the Tashrif platform (Hajj & Umrah seasonal recruitment).

---

## 1. `users` — Auth & Core Identity

```sql
CREATE TABLE users (
  id            BIGSERIAL PRIMARY KEY,
  national_id   VARCHAR(20) UNIQUE NOT NULL,
  password_hash VARCHAR(255) NOT NULL,
  name          VARCHAR(100) NOT NULL,
  email         VARCHAR(100) NOT NULL,
  phone         VARCHAR(20),
  type          VARCHAR(20) NOT NULL CHECK (type IN ('individual', 'entity')),
  gender        VARCHAR(10) CHECK (gender IN ('male', 'female')),
  nationality   VARCHAR(50),
  avatar_url    VARCHAR(500),
  created_at    TIMESTAMPTZ DEFAULT NOW(),
  updated_at    TIMESTAMPTZ DEFAULT NOW()
);
```

---

## 2. `individual_profiles` — Extended Individual Data

```sql
CREATE TABLE individual_profiles (
  id                    BIGSERIAL PRIMARY KEY,
  user_id               BIGINT UNIQUE NOT NULL REFERENCES users(id),
  birth_date            DATE,
  city                  VARCHAR(100),
  zone                  VARCHAR(100),
  district              VARCHAR(100),
  street                VARCHAR(200),
  zipcode               VARCHAR(20),
  job_title             VARCHAR(200),
  profile_completion_pct SMALLINT DEFAULT 0,
  created_at            TIMESTAMPTZ DEFAULT NOW(),
  updated_at            TIMESTAMPTZ DEFAULT NOW()
);
```

---

## 3. `qualifications` — Academic History

```sql
CREATE TABLE qualifications (
  id              BIGSERIAL PRIMARY KEY,
  user_id         BIGINT NOT NULL REFERENCES users(id),
  type            VARCHAR(100) NOT NULL,
  specialization  VARCHAR(200),
  institution     VARCHAR(200),
  graduation_year SMALLINT,
  grade           VARCHAR(50),
  created_at      TIMESTAMPTZ DEFAULT NOW()
);
```

---

## 4. `experiences` — Work History

```sql
CREATE TABLE experiences (
  id              BIGSERIAL PRIMARY KEY,
  user_id         BIGINT NOT NULL REFERENCES users(id),
  job_title       VARCHAR(200) NOT NULL,
  employer        VARCHAR(200) NOT NULL,
  duration        VARCHAR(100),
  location        VARCHAR(200),
  is_current      BOOLEAN DEFAULT FALSE,
  start_date      DATE,
  end_date        DATE,
  created_at      TIMESTAMPTZ DEFAULT NOW()
);
```

---

## 5. `cvs` — Uploaded CVs/Resumes

```sql
CREATE TABLE cvs (
  id              BIGSERIAL PRIMARY KEY,
  user_id         BIGINT NOT NULL REFERENCES users(id),
  file_name       VARCHAR(255) NOT NULL,
  file_path       VARCHAR(500) NOT NULL,
  file_size       BIGINT,
  is_default      BOOLEAN DEFAULT FALSE,
  uploaded_at     TIMESTAMPTZ DEFAULT NOW()
);
```

---

## 6. `bank_accounts` — Financial Info

```sql
CREATE TABLE bank_accounts (
  id              BIGSERIAL PRIMARY KEY,
  user_id         BIGINT NOT NULL REFERENCES users(id),
  iban            VARCHAR(34) NOT NULL,
  bank_name       VARCHAR(200) NOT NULL,
  iban_status     VARCHAR(50) DEFAULT 'pending',
  account_status  VARCHAR(50) DEFAULT 'active',
  created_at      TIMESTAMPTZ DEFAULT NOW(),
  updated_at      TIMESTAMPTZ DEFAULT NOW()
);
```

---

## 7. `entity_profiles` — Company Data

```sql
CREATE TABLE entity_profiles (
  id              BIGSERIAL PRIMARY KEY,
  user_id         BIGINT UNIQUE NOT NULL REFERENCES users(id),
  company_field   VARCHAR(200),
  sector          VARCHAR(200),
  company_size    VARCHAR(50),
  commercial_reg  VARCHAR(50),
  country         VARCHAR(100),
  city            VARCHAR(100),
  zone            VARCHAR(100),
  district        VARCHAR(100),
  street          VARCHAR(200),
  zipcode         VARCHAR(20),
  website         VARCHAR(500),
  facebook_url    VARCHAR(500),
  twitter_url     VARCHAR(500),
  youtube_url     VARCHAR(500),
  logo_url        VARCHAR(500),
  profile_completion_pct SMALLINT DEFAULT 0,
  created_at      TIMESTAMPTZ DEFAULT NOW(),
  updated_at      TIMESTAMPTZ DEFAULT NOW()
);
```

---

## 8. `contact_persons` — Entity Contact People

```sql
CREATE TABLE contact_persons (
  id              BIGSERIAL PRIMARY KEY,
  entity_id       BIGINT NOT NULL REFERENCES entity_profiles(id) ON DELETE CASCADE,
  name            VARCHAR(100) NOT NULL,
  role            VARCHAR(200),
  nationality     VARCHAR(50),
  phone           VARCHAR(20),
  email           VARCHAR(100),
  is_primary      BOOLEAN DEFAULT FALSE,
  created_at      TIMESTAMPTZ DEFAULT NOW()
);
```

---

## 9. `jobs` — Job Listings

```sql
CREATE TABLE jobs (
  id              BIGSERIAL PRIMARY KEY,
  entity_id       BIGINT NOT NULL REFERENCES users(id),
  title           VARCHAR(200) NOT NULL,
  description     TEXT NOT NULL,
  location        VARCHAR(200) NOT NULL CHECK (location IN ('makkah','madinah','jeddah','taif','mina','arafat','muzdalifah','rabigh','khulais','bahrah','jumum','allith','qunfudhah','yanbu','badr','riyadh')),
  work_type       VARCHAR(50) NOT NULL CHECK (work_type IN ('full-time', 'part-time', 'seasonal')),
  target          VARCHAR(50),
  vacancies       INTEGER NOT NULL,
  qualification   VARCHAR(100),
  salary_min      NUMERIC(10,2),
  salary_max      NUMERIC(10,2),
  salary_text     VARCHAR(200),
  gender          VARCHAR(20) CHECK (gender IN ('male', 'female', 'both')),
  hours           VARCHAR(100),
  duration        VARCHAR(100),
  status          VARCHAR(20) DEFAULT 'draft' CHECK (status IN ('draft', 'active', 'closed')),
  publish_date    DATE,
  end_date        DATE,
  created_at      TIMESTAMPTZ DEFAULT NOW(),
  updated_at      TIMESTAMPTZ DEFAULT NOW()
);
```

---

## 10. `job_benefits` — Benefits

```sql
CREATE TABLE job_benefits (
  id            BIGSERIAL PRIMARY KEY,
  job_id        BIGINT NOT NULL REFERENCES jobs(id) ON DELETE CASCADE,
  benefit_text  VARCHAR(500) NOT NULL,
  sort_order    SMALLINT DEFAULT 0
);
```

---

## 11. `job_responsibilities` — Duties

```sql
CREATE TABLE job_responsibilities (
  id              BIGSERIAL PRIMARY KEY,
  job_id          BIGINT NOT NULL REFERENCES jobs(id) ON DELETE CASCADE,
  responsibility  VARCHAR(500) NOT NULL,
  sort_order      SMALLINT DEFAULT 0
);
```

---

## 12. `job_conditions` — Requirements

```sql
CREATE TABLE job_conditions (
  id              BIGSERIAL PRIMARY KEY,
  job_id          BIGINT NOT NULL REFERENCES jobs(id) ON DELETE CASCADE,
  condition_text  VARCHAR(500) NOT NULL,
  sort_order      SMALLINT DEFAULT 0
);
```

---

## 13. `applications` — Job Applications

```sql
CREATE TABLE applications (
  id              BIGSERIAL PRIMARY KEY,
  job_id          BIGINT NOT NULL REFERENCES jobs(id),
  user_id         BIGINT NOT NULL REFERENCES users(id),
  cv_id           BIGINT REFERENCES cvs(id),
  qualification   VARCHAR(200),
  experience      TEXT,
  cover_letter    TEXT,
  status          VARCHAR(20) DEFAULT 'new' CHECK (status IN (
                    'new', 'shortlisted', 'interview', 'contract_sent', 'accepted', 'refused'
                  )),
  created_at      TIMESTAMPTZ DEFAULT NOW(),
  updated_at      TIMESTAMPTZ DEFAULT NOW(),
  UNIQUE(job_id, user_id)
);
```

---

## 14. `interviews` — Scheduled Interviews

```sql
CREATE TABLE interviews (
  id              BIGSERIAL PRIMARY KEY,
  application_id  BIGINT UNIQUE NOT NULL REFERENCES applications(id),
  job_id          BIGINT NOT NULL REFERENCES jobs(id),
  user_id         BIGINT NOT NULL REFERENCES users(id),
  entity_id       BIGINT NOT NULL REFERENCES users(id),
  method          VARCHAR(20) NOT NULL CHECK (method IN ('in-person', 'phone', 'video')),
  date            DATE NOT NULL,
  time            TIME NOT NULL,
  location        VARCHAR(300),
  link            VARCHAR(500),
  notes           TEXT,
  status          VARCHAR(20) DEFAULT 'scheduled' CHECK (status IN ('scheduled', 'completed', 'cancelled')),
  attendance      VARCHAR(20) DEFAULT 'pending' CHECK (attendance IN ('pending', 'present', 'absent')),
  created_at      TIMESTAMPTZ DEFAULT NOW(),
  updated_at      TIMESTAMPTZ DEFAULT NOW()
);
```

---

## 15. `contracts` — Employment Contracts

```sql
CREATE TABLE contracts (
  id              BIGSERIAL PRIMARY KEY,
  application_id  BIGINT UNIQUE NOT NULL REFERENCES applications(id),
  job_id          BIGINT NOT NULL REFERENCES jobs(id),
  user_id         BIGINT NOT NULL REFERENCES users(id),
  entity_id       BIGINT NOT NULL REFERENCES users(id),
  file_url        VARCHAR(500),
  file_size       BIGINT,
  status          VARCHAR(20) DEFAULT 'sent' CHECK (status IN ('sent', 'signed', 'rejected')),
  signed_at       TIMESTAMPTZ,
  created_at      TIMESTAMPTZ DEFAULT NOW(),
  updated_at      TIMESTAMPTZ DEFAULT NOW()
);
```

---

## 16. `notifications` — User Notifications

```sql
CREATE TABLE notifications (
  id              BIGSERIAL PRIMARY KEY,
  user_id         BIGINT NOT NULL REFERENCES users(id),
  title           VARCHAR(300) NOT NULL,
  body            TEXT,
  type            VARCHAR(50),
  reference_id    BIGINT,
  reference_type  VARCHAR(50),
  is_read         BOOLEAN DEFAULT FALSE,
  created_at      TIMESTAMPTZ DEFAULT NOW()
);
```

---

## 17. `reviews` — Applicant Evaluation

```sql
CREATE TABLE reviews (
  id              BIGSERIAL PRIMARY KEY,
  application_id  BIGINT NOT NULL REFERENCES applications(id),
  reviewer_id     BIGINT NOT NULL REFERENCES users(id),
  rating          SMALLINT CHECK (rating BETWEEN 1 AND 5),
  notes           TEXT,
  decision        VARCHAR(20) CHECK (decision IN ('pass', 'fail', 'hold')),
  created_at      TIMESTAMPTZ DEFAULT NOW()
);
```

---

## Entity Relationships

```
users (1) ──< (1) individual_profiles     [user_id FK]
users (1) ──< (N) qualifications          [user_id FK]
users (1) ──< (N) experiences             [user_id FK]
users (1) ──< (N) cvs                     [user_id FK]
users (1) ──< (N) bank_accounts           [user_id FK]
users (1) ──< (1) entity_profiles         [user_id FK]
entity_profiles (1) ──< (N) contact_persons [entity_id FK]
users (1) ──< (N) jobs                    [entity_id FK]
jobs (1) ──< (N) job_benefits             [job_id FK]
jobs (1) ──< (N) job_responsibilities     [job_id FK]
jobs (1) ──< (N) job_conditions           [job_id FK]
users (1) ──< (N) applications            [user_id FK]
jobs (1) ──< (N) applications             [job_id FK]
applications (1) ──< (1) interviews       [application_id FK]
applications (1) ──< (1) contracts        [application_id FK]
applications (1) ──< (N) reviews          [application_id FK]
users (1) ──< (N) notifications           [user_id FK]
```

---

## Stats Queries

### Individual Dashboard

```sql
SELECT
  (SELECT COUNT(*) FROM applications WHERE user_id = :uid) AS total_applications,
  (SELECT COUNT(*) FROM applications WHERE user_id = :uid AND status IN ('new', 'shortlisted')) AS pending_apps,
  (SELECT COUNT(*) FROM interviews WHERE user_id = :uid) AS total_interviews,
  (SELECT COUNT(*) FROM contracts WHERE user_id = :uid AND status = 'signed') AS signed_contracts;
```

### Entity Dashboard

```sql
SELECT
  (SELECT COUNT(*) FROM jobs WHERE entity_id = :uid) AS total_jobs,
  (SELECT COUNT(*) FROM jobs WHERE entity_id = :uid AND status = 'active') AS active_jobs,
  (SELECT COUNT(*) FROM applications a JOIN jobs j ON a.job_id = j.id WHERE j.entity_id = :uid) AS total_applicants;
```

---

## Indexes (Recommended)

```sql
CREATE INDEX idx_users_national_id ON users(national_id);
CREATE INDEX idx_users_type ON users(type);
CREATE INDEX idx_jobs_entity_id ON jobs(entity_id);
CREATE INDEX idx_jobs_status ON jobs(status);
CREATE INDEX idx_applications_job_id ON applications(job_id);
CREATE INDEX idx_applications_user_id ON applications(user_id);
CREATE INDEX idx_applications_status ON applications(status);
CREATE INDEX idx_interviews_user_id ON interviews(user_id);
CREATE INDEX idx_interviews_entity_id ON interviews(entity_id);
CREATE INDEX idx_contracts_user_id ON contracts(user_id);
CREATE INDEX idx_contracts_entity_id ON contracts(entity_id);
CREATE INDEX idx_notifications_user_id ON notifications(user_id);
CREATE INDEX idx_notifications_is_read ON notifications(is_read);
```
