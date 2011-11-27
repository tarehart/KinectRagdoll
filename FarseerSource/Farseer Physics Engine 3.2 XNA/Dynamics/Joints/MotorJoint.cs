using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FarseerPhysics.Dynamics.Joints
{
    [DataContract(Name = "MotorJoint", Namespace = "http://www.imcool.com")]
    [KnownType(typeof(FixedRevoluteJoint))]
    [KnownType(typeof(RevoluteJoint))]
    public abstract class MotorJoint : Joint
    {
        [DataMember()]
        protected bool _enableMotor;
        [DataMember()]
        protected float _maxMotorTorque;
        protected float _motorImpulse;
        protected float _motorMass; // effective mass for motor/limit angular constraint.
        [DataMember()]
        protected float _motorSpeed;

        public MotorJoint(Body a, Body b)
            : base(a, b)
        {

        }

        public MotorJoint(Body body) : base(body)
        {

        }

        public MotorJoint()
            : base()
        {
        }


        /// <summary>
        /// Is the joint motor enabled?
        /// </summary>
        /// <value><c>true</c> if [motor enabled]; otherwise, <c>false</c>.</value>
        public bool MotorEnabled
        {
            get { return _enableMotor; }
            set
            {
                WakeBodies();
                _enableMotor = value;
            }
        }

        /// <summary>
        /// Set the motor speed in radians per second.
        /// </summary>
        /// <value>The speed.</value>
        public float MotorSpeed
        {
            set
            {
                WakeBodies();
                _motorSpeed = value;
            }
            get { return _motorSpeed; }
        }

        /// <summary>
        /// Set the maximum motor torque, usually in N-m.
        /// </summary>
        /// <value>The torque.</value>
        public float MaxMotorTorque
        {
            set
            {
                WakeBodies();
                _maxMotorTorque = value;
            }
            get { return _maxMotorTorque; }
        }

        /// <summary>
        /// Get the current motor torque, usually in N-m.
        /// </summary>
        /// <value></value>
        public float MotorTorque
        {
            get { return _motorImpulse; }
            set
            {
                WakeBodies();
                _motorImpulse = value;
            }
        }

        
    }
}
