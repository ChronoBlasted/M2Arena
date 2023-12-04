let InitModule: nkruntime.InitModule = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, initializer: nkruntime.Initializer) {

    // Set up hooks.
    initializer.registerAfterAuthenticateDevice(afterAuthenticate);
    initializer.registerAfterAuthenticateEmail(afterAuthenticate);

    logger.info('XXXXXXXXXXXXXXXXXXXX - M2Arena TypeScript loaded - XXXXXXXXXXXXXXXXXXXX');
}



function afterAuthenticate(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, data: nkruntime.Session) {
    if (!data.created) {
        logger.info('User with id: %s account data already existing', ctx.userId);
        return
    }

    let user_id = ctx.userId;
    let username = "Player_" + ctx.username;
    let metadata = {
        battle_pass: false,
        win: 0,
        loose: 0,
    };
    let displayName = "NewPlayer";
    let timezone = null;
    let location = null;
    let langTag = "EN";
    let avatarUrl = null;

    try {
        nk.accountUpdateId(user_id, username, displayName, timezone, location, langTag, avatarUrl, metadata);
    } catch (error) {
        logger.error('Error init update account : %s', error);
    }

    let changeset = {
        'coins': 1000,
        'gems': 100,
        'trophies': 0,
    };

    try {
        nk.walletUpdate(user_id, changeset);
    } catch (error) {
        logger.error('Error init new wallet : %s', error);
    }


    logger.debug('new user id: %s account data initialised', ctx.userId);
}