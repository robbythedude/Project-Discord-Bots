# Discord Webhook Bot

The Discord Webhook Bot is a bot designed to periodically provide basic information about crypto currencies to a Discord chat. The Discord Webhook Bot was designed to be easily hosted on Amazon Web Services (AWS) and provided reliable info from various crypto APIs.

### How

The bot is a NodeJS application meant to be ran as a Lambda fucntion in AWS. Combined with AWS CloudWatch service, the bot will function on a schedule and post information as frequently as needed. If the frequency combined with complexity of the bot stay minimal, this bot can be hosted for free under the AWS Free Tier package! Great!

### Install

Assuming basic experience with AWS and their services, the setup is really straight forward.

* Create a new **Lambda** function on **AWS**  (Name it whatever you want...)
* Add the **index.js** and **config.json** files found in this repo to the **Lambda** function
* Set the correct values in the **config.json** file
* Save, Run, and Verify in Discord!
* Create a new **CloudWatch** trigger that runs the **Lambda** function perdiocally.
* **_Voila_**! The bot shall now post to Discord periodically with crypto currency information!