-- Adds patient image storage and keeps user image storage compatible.
-- Run this script once on your HospitalManagementSystem database.

ALTER TABLE `patients`
    ADD COLUMN IF NOT EXISTS `ProfileImage` LONGBLOB NULL;

ALTER TABLE `userdetails`
    MODIFY COLUMN `ProfileImage` LONGBLOB NULL;
