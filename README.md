# Beacon-Clinical-Guidelines

## Introduction
Beacon Clinical Guidelines is an open source web and mobile application created by University Hospitals Plymouth NHS Trust. The goal is to provide succinct guidelines for use by Trust staff. The initial focus has been upon local departmental information that act as memory joggers and points to more detailed proceses and procedures.

The solution is intended to be configured and installed by each organisation that wishes to use it, with content relevant only to that organisation. It currently consists of a web application, API and mobile (hybrid) application.

## Disclaimer
Beacon Clinical Guidelines is provided under an GNU Affero GPL v3.0 (AGPL v3.0) license and all terms of that license apply (https://www.gnu.org/licenses/agpl-3.0.en.html). University Hospitals Plymouth NHS Trust does not accept any responsibility for loss or damage to any person, property or reputation as a result of using the software or code. No warranty is provided by any party, implied or otherwise. This software and code is not guaranteed safe to use in a clinical environment; any user is advised to undertake a safety assessment to confirm that deployment matches local clinical safety requirements.

## Dev Setup
Beacon is a dotnet 4.5.2 API with an Angular JS app within.

It requires the following variables to be replaced in Web.Config:
SOURCE
DATABASE
mailFromAddress
mailUsername
mailPassword
mailSMTPServer
testEmails

Beacaon is configured to work with multiple environments. Using individual Web.Config files fore each envrionment to overwrite the master file on build.
This is configured in the proejcts build events.

Further information on how to compile, install and configure the solution will be added in due course.