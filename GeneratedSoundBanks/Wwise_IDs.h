/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID ENTERMAINMENU = 3531603487U;
        static const AkUniqueID ENTERPAUSEMENU = 3602428572U;
        static const AkUniqueID ONCLICK = 21544190U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace GAMESTATUS
        {
            static const AkUniqueID GROUP = 1045871717U;

            namespace STATE
            {
                static const AkUniqueID INGAME = 984691642U;
                static const AkUniqueID INMENU = 3374585465U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace GAMESTATUS

        namespace NPC
        {
            static const AkUniqueID GROUP = 662417162U;

            namespace STATE
            {
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID NOTTALKING = 3168108838U;
                static const AkUniqueID TALKING = 3375731105U;
            } // namespace STATE
        } // namespace NPC

        namespace PLAYERSTATE
        {
            static const AkUniqueID GROUP = 3285234865U;

            namespace STATE
            {
                static const AkUniqueID DISCOVERY = 3663283413U;
                static const AkUniqueID DOUBLEJUMP = 1308315758U;
                static const AkUniqueID LOBBY = 290285391U;
                static const AkUniqueID MEGAPOING = 1458596172U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace PLAYERSTATE

        namespace PUZZLE
        {
            static const AkUniqueID GROUP = 1780448749U;

            namespace STATE
            {
                static const AkUniqueID DOINGPUZZLE = 830272446U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID NOTDOINGPUZZLE = 4192918885U;
            } // namespace STATE
        } // namespace PUZZLE

    } // namespace STATES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID COMBATINTENSITY = 1253610732U;
        static const AkUniqueID SFXVOLUME = 988953028U;
        static const AkUniqueID SS_AIR_FEAR = 1351367891U;
        static const AkUniqueID SS_AIR_FREEFALL = 3002758120U;
        static const AkUniqueID SS_AIR_FURY = 1029930033U;
        static const AkUniqueID SS_AIR_MONTH = 2648548617U;
        static const AkUniqueID SS_AIR_PRESENCE = 3847924954U;
        static const AkUniqueID SS_AIR_RPM = 822163944U;
        static const AkUniqueID SS_AIR_SIZE = 3074696722U;
        static const AkUniqueID SS_AIR_STORM = 3715662592U;
        static const AkUniqueID SS_AIR_TIMEOFDAY = 3203397129U;
        static const AkUniqueID SS_AIR_TURBULENCE = 4160247818U;
        static const AkUniqueID VOLUME = 2415836739U;
    } // namespace GAME_PARAMETERS

    namespace BUSSES
    {
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
