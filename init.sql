DO
$$
BEGIN
   IF NOT EXISTS (
      SELECT FROM pg_database
      WHERE datname = 'drillingcore'
   ) THEN
      CREATE DATABASE drillingcore;
   END IF;
END
$$;
