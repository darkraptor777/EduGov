using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace toolbox
{
    
    public enum characteristic { Technocrat, Plutocrat, Stratocrat, Theocrat, Unionist }; //science, money, military, religion, union member

    public enum chamber { Upper, Lower, Unified };

    public enum representativeSystem { Delegate, Trustee };

    public enum gender { Male, Female };

    public enum platform { Liberalism, Conservatism, Socialism, Nationalism, Fascism, Communism, Syndicalism, Environmentalism, Internationalism, Isolationist, Progressism, Libertarianism, SocialDemocratic };

    public enum legislativeSystem { unicameral, bicameral };

    public enum executiveSystem { Parliamentary, Presidential };

    public enum electoralSystem { FPTP, IRV };

    public enum vote { Nay, Yay, Abstain };
}
